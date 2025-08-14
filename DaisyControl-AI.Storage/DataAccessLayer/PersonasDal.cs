using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.Exceptions;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Response.Personas;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public class PersonasDal : IPersonasDal
    {
        private const int NbMsToDelayAfterProvisionException = 5000;
        private const int NbMsUnreachableDbOnStartup = 30000;
        private string personaTableName = "DaisyControl-Personas";
        private AWSCredentials AWSCredentials = new BasicAWSCredentials("local", "local");
        private string DynamoDBUri = "http://127.0.0.1:8822";
        private IAmazonDynamoDB dynamoDBClient = null;

        public PersonasDal()
        {
            dynamoDBClient = InitClient();
            InitDatabase().Wait();
            CleanUpPersonasTable().Wait();
        }

        private async Task InitDatabase()
        {
            var config = CommonConfigurationManager.ReloadConfig();

            // Create a cancellationToken with a predefinite waiting time to avoid infinitely waiting on the DB here. If it's down, we want to EXIT instead of freezing to avoid scaling snowball
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    cancellationTokenSource.CancelAfter(NbMsUnreachableDbOnStartup);
                    ListTablesResponse tables = await dynamoDBClient.ListTablesAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                    // Personas Table
                    if (!tables.TableNames.Contains(personaTableName))
                    {
                        await CreateTable(new CreateTableRequest(personaTableName, new List<KeySchemaElement>
                        {
                            new KeySchemaElement("personaId", KeyType.HASH),
                            //new KeySchemaElement("status", KeyType.RANGE),
                        }, new List<AttributeDefinition>
                        {
                            new AttributeDefinition("personaId", ScalarAttributeType.S),
                            new AttributeDefinition("status", ScalarAttributeType.S),
                            new AttributeDefinition("pendingInferenceTasksCounter", ScalarAttributeType.N),
                        }, new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 1000,
                            WriteCapacityUnits = 1000,
                        })
                        {
                            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                            {
                                new()
                                {
                                    IndexName = config.StorageConfiguration.PersonasWithInferenceTasksIndexName,
                                    KeySchema = new List<KeySchemaElement>
                                    {
                                        new("status", KeyType.HASH),
                                        new("pendingInferenceTasksCounter", KeyType.RANGE),
                                    },
                                    Projection = new Projection
                                    {
                                        ProjectionType = ProjectionType.ALL,
                                    },
                                    ProvisionedThroughput = new ProvisionedThroughput
                                    {
                                        ReadCapacityUnits = 1000,
                                        WriteCapacityUnits = 1000,
                                    },
                                },
                            },
                        });
                    }

                    // Personas Table
                    if (!tables.TableNames.Contains(personaTableName))
                    {
                        await CreateTable(new CreateTableRequest(personaTableName, new List<KeySchemaElement>
                        {
                            new KeySchemaElement("personaId", KeyType.HASH),
                        }, new List<AttributeDefinition>
                        {
                            new AttributeDefinition("personaId", ScalarAttributeType.S),
                            new AttributeDefinition("status", ScalarAttributeType.S),
                            new AttributeDefinition("pendingInferenceTasksCounter", ScalarAttributeType.N),
                        }, new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 1000,
                            WriteCapacityUnits = 1000,
                        })
                        {
                            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                            {
                                new()
                                {
                                    IndexName = config.StorageConfiguration.PersonasWithInferenceTasksIndexName,
                                    KeySchema = new List<KeySchemaElement>
                                    {
                                        new("status", KeyType.HASH),
                                        new("pendingInferenceTasksCounter", KeyType.RANGE),
                                    },
                                    Projection = new Projection
                                    {
                                        ProjectionType = ProjectionType.ALL,
                                    },
                                    ProvisionedThroughput = new ProvisionedThroughput
                                    {
                                        ReadCapacityUnits = 1000,
                                        WriteCapacityUnits = 1000,
                                    },
                                },
                            },
                        });
                    }
                } catch (OperationCanceledException e)
                {
                    string errMessage = $"The database was unreachable for [{NbMsUnreachableDbOnStartup}] ms on startup... aborting.";
                    LoggingManager.LogToFile("a904301c-2a87-4f6f-bb2d-a626aa3d2892", errMessage);
                    throw new CommonException("3fb7f9c1-0466-4f9c-bfab-48b69c75d7e4", errMessage, e);
                } finally
                {
                    cancellationTokenSource.Dispose();
                }
            }
        }

        private async Task CleanUpPersonasTable()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    cancellationTokenSource.CancelAfter(NbMsUnreachableDbOnStartup);
                    ListTablesResponse tables = await dynamoDBClient.ListTablesAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                    if (tables.TableNames.Contains(personaTableName))
                    {
                        LoggingManager.LogToFile("044fd287-5184-4d57-8389-c8fbcdfb8353", $"Cleaning up Personas table...", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                        // Cleanup personas stuck with Working status
                        var stuckPersonas = await TryGetPersonasWithWorkingStatusAsync(1000);

                        if (stuckPersonas != null)
                        {
                            var now = DateTime.UtcNow;
                            foreach (var persona in stuckPersonas.Personas.Where(w => (now - w.LastModifiedAtUtc).TotalMilliseconds >= 15000))
                            {
                                persona.Status = UserStatus.Ready;
                                var json = JsonSerializer.Serialize(persona);
                                var personaAsDto = JsonSerializer.Deserialize<DaisyControlUpdatePersonaRequestDto>(json);
                                await TryUpdatePersonaAsync(personaAsDto);
                            }
                        }
                    }

                } catch (Exception e)
                {
                    LoggingManager.LogToFile("95eedae2-045b-408d-8d0b-8aa7625676d3", $"Error. Couldn't clean up Personas table when Initializing Database!");
                }
            }
        }

        private async Task CreateTable(CreateTableRequest createTableRequest) => await dynamoDBClient.CreateTableAsync(createTableRequest).ConfigureAwait(false);

        private IAmazonDynamoDB InitClient()
        {
            return new AmazonDynamoDBClient(AWSCredentials, new AmazonDynamoDBConfig()
            {
                Timeout = new TimeSpan(0, 0, 10),
                ServiceURL = DynamoDBUri,
            });
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetPersonasResponseDto> TryGetPersonasWithWorkingStatusAsync(int limitRows)
        {
            var config = CommonConfigurationManager.ReloadConfig();

            try
            {
                var queryResponse = await dynamoDBClient.QueryAsync(new QueryRequest
                {
                    TableName = personaTableName,
                    Limit = limitRows,
                    ConsistentRead = false,
                    KeyConditionExpression = "#status = :status",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.Working.ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetPersonaResponseDto> PersonasCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var persona = JsonSerializer.Deserialize<DaisyControlGetPersonaResponseDto>(jsonResponse);
                    PersonasCollection.Add(persona);
                }

                return new DaisyControlGetPersonasResponseDto
                {
                    Personas = PersonasCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("cbb7ec69-a9a5-4079-b798-a37c31512aa1", $"Unhandled exception when querying database to fetch personas with working status. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetPersonaResponseDto> TryGetPersonaAsync(string personaId)
        {
            if (string.IsNullOrWhiteSpace(personaId))
            {
                throw new ArgumentNullException(nameof(personaId));
            }

            try
            {
                var personaItemsResult = await dynamoDBClient.GetItemAsync(new GetItemRequest
                {
                    TableName = personaTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "personaId", new AttributeValue { S = personaId } },
                    },
                }).ConfigureAwait(false);

                if (personaItemsResult.Item == null || personaItemsResult.Item.Count <= 0)
                {
                    return null; // Item not found
                }

                var responseDocument = Document.FromAttributeMap(personaItemsResult.Item);

                var jsonResponse = responseDocument.ToJson();
                var personaDto = JsonSerializer.Deserialize<DaisyControlGetPersonaResponseDto>(jsonResponse);

                return personaDto;

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("3a7d0b58-4c83-425a-8b88-32d33f708188", $"Unhandled exception when querying database. Failed to get Persona matching PersonaId [{personaId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlAddPersonaRequestDto> TryAddPersonaAsync(DaisyControlAddPersonaRequestDto daisyControlAddPersonaDto)
        {
            if (daisyControlAddPersonaDto == null)
            {
                throw new ArgumentNullException(nameof(daisyControlAddPersonaDto));
            }

            var utcNow = DateTime.UtcNow;
            daisyControlAddPersonaDto.Status = UserStatus.Ready;
            daisyControlAddPersonaDto.CreatedAtUtc = utcNow;
            daisyControlAddPersonaDto.LastModifiedAtUtc = utcNow;
            daisyControlAddPersonaDto.Revision = 0;

            try
            {
                var personaItemsResult = await dynamoDBClient.PutItemAsync(new PutItemRequest
                {
                    TableName = personaTableName,
                    Item = daisyControlAddPersonaDto.ToDocument(null).ToAttributeMap(),
                    ConditionExpression = "personaId <> :personaId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":personaId", new AttributeValue { S = daisyControlAddPersonaDto.Id } },
                    },
                }).ConfigureAwait(false);

                return daisyControlAddPersonaDto;
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("957eeaf4-fa47-4088-b80d-e78c7ca0abf0", $"Persona was created by another instance. Persona [{daisyControlAddPersonaDto.PersonaInfo.Username}] won't be created to avoid duplicates.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("4844ca76-383c-4d94-b2c0-f3fa0fbdf342", $"Unhandled exception when querying database. Failed to add Persona of Name [{daisyControlAddPersonaDto.PersonaInfo.Username}] to storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlUpdatePersonaRequestDto> TryUpdatePersonaAsync(DaisyControlUpdatePersonaRequestDto daisyControlUpdatePersonaDto)
        {
            if (daisyControlUpdatePersonaDto == null)
            {
                throw new ArgumentNullException(nameof(daisyControlUpdatePersonaDto));
            }

            long requestLastRevision = daisyControlUpdatePersonaDto.Revision;

            var utcNow = DateTime.UtcNow;
            daisyControlUpdatePersonaDto.LastModifiedAtUtc = utcNow;
            daisyControlUpdatePersonaDto.Revision++;

            try
            {
                var personaItemsResult = await dynamoDBClient.PutItemAsync(new PutItemRequest
                {
                    TableName = personaTableName,
                    Item = daisyControlUpdatePersonaDto.ToDocument(null).ToAttributeMap(),
                    ConditionExpression = "#revision = :revision",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#revision", "revision" },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":revision", new AttributeValue { N = requestLastRevision.ToString() } },
                    },
                }).ConfigureAwait(false);

                return daisyControlUpdatePersonaDto;
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("c2dfd9a1-09c9-4a9b-80f8-55ea7b5e8d70", $"Revision didn't match. Persona [{daisyControlUpdatePersonaDto.PersonaInfo.Username}] won't be updated.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("673a4204-3c83-4717-915c-65724841645c", $"Unhandled exception when querying database. Failed to update Persona [{daisyControlUpdatePersonaDto.Id}] in storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> TryDeletePersonaAsync(string personaId)
        {
            if (string.IsNullOrWhiteSpace(personaId))
            {
                throw new ArgumentNullException(nameof(personaId));
            }

            try
            {
                var response = await dynamoDBClient.DeleteItemAsync(new DeleteItemRequest
                {
                    TableName = personaTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "personaId", new AttributeValue { S = personaId } },
                    },
                }).ConfigureAwait(false);

                return response.HttpStatusCode != HttpStatusCode.NoContent;
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("bcd3fe41-f87d-4434-b693-b65390a20051", $"Unhandled exception when querying database. Failed to delete Persona matching PersonaId [{personaId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }
    }
}
