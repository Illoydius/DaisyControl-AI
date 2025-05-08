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
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public class DaisyControlDal : IDaisyControlDal
    {
        private const int NbMsToDelayAfterProvisionException = 5000;
        private const int NbMsUnreachableDbOnStartup = 30000;
        private string userTableName = "DaisyControl-Users";
        private AWSCredentials AWSCredentials = new BasicAWSCredentials("local", "local");
        private string DynamoDBUri = "http://127.0.0.1:8822";
        private IAmazonDynamoDB dynamoDBClient = null;

        public DaisyControlDal()
        {
            dynamoDBClient = InitClient();
            InitDatabase().Wait();
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

                    if (!tables.TableNames.Contains(userTableName))
                    {
                        await CreateTable(new CreateTableRequest(userTableName, new List<KeySchemaElement>
                        {
                            new KeySchemaElement("userId", KeyType.HASH),
                        }, new List<AttributeDefinition>
                        {
                            new AttributeDefinition("userId", ScalarAttributeType.S),
                            new AttributeDefinition("status", ScalarAttributeType.S),
                            new AttributeDefinition("nbUnprocessedMessages", ScalarAttributeType.N),
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
                                    IndexName = config.StorageConfiguration.UsersWithMessagesToProcessIndexName,
                                    KeySchema = new List<KeySchemaElement>
                                    {
                                        new("status", KeyType.HASH),
                                        new("nbUnprocessedMessages", KeyType.RANGE),
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
                }
                catch (OperationCanceledException e)
                {
                    string errMessage = $"The database was unreachable for [{NbMsUnreachableDbOnStartup}] ms on startup... aborting.";
                    LoggingManager.LogToFile("a904301c-2a87-4f6f-bb2d-a626aa3d2892", errMessage);
                    throw new CommonException("3fb7f9c1-0466-4f9c-bfab-48b69c75d7e4", errMessage, e);
                }
                finally
                {
                    cancellationTokenSource.Dispose();
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
        public async Task<DaisyControlStorageUserDto> TryGetUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            try
            {
                var userItemsResult = await dynamoDBClient.GetItemAsync(new GetItemRequest
                {
                    TableName = userTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "userId", new AttributeValue { S = userId } },
                    },
                }).ConfigureAwait(false);

                if (userItemsResult.Item == null || userItemsResult.Item.Count <= 0)
                {
                    return null; // Item not found
                }

                var responseDocument = Document.FromAttributeMap(userItemsResult.Item);

                var jsonResponse = responseDocument.ToJson();
                var userDto = JsonSerializer.Deserialize<DaisyControlStorageUserDto>(jsonResponse);

                return userDto;

            }
            catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            }
            catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("3a7d0b58-4c83-425a-8b88-32d33f708188", $"Unhandled exception when querying database. Failed to get User matching UserId [{userId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlAddUserRequestDto> TryAddUserAsync(DaisyControlAddUserRequestDto daisyControlAddUserDto)
        {
            if (daisyControlAddUserDto == null)
            {
                throw new ArgumentNullException(nameof(daisyControlAddUserDto));
            }

            var utcNow = DateTime.UtcNow;
            daisyControlAddUserDto.CreatedAtUtc = utcNow;
            daisyControlAddUserDto.LastModifiedAtUtc = utcNow;
            daisyControlAddUserDto.Revision = 0;

            try
            {
                var userItemsResult = await dynamoDBClient.PutItemAsync(new PutItemRequest
                {
                    TableName = userTableName,
                    Item = daisyControlAddUserDto.ToDocument(null).ToAttributeMap(),
                    ConditionExpression = "userId <> :userId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":userId", new AttributeValue { S = daisyControlAddUserDto.Id } },
                    },
                }).ConfigureAwait(false);

                return daisyControlAddUserDto;
            }
            catch (ConditionalCheckFailedException)
            {
                throw new CommonException("fb23d483-9131-45f8-90f4-3bd192bfe520", $"User was created by another instance. User [{daisyControlAddUserDto.Username}] won't be created to avoid duplicates.");
            }
            catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            }
            catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("008d82d9-4181-4466-be67-07533b912df0", $"Unhandled exception when querying database. Failed to add User of Name [{daisyControlAddUserDto.Username}] to storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlUpdateUserRequestDto> TryUpdateUserAsync(DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto)
        {
            if (daisyControlUpdateUserDto == null)
            {
                throw new ArgumentNullException(nameof(daisyControlUpdateUserDto));
            }

            long requestLastRevision = daisyControlUpdateUserDto.Revision;

            var utcNow = DateTime.UtcNow;
            daisyControlUpdateUserDto.LastModifiedAtUtc = utcNow;
            daisyControlUpdateUserDto.Revision++;

            try
            {
                var userItemsResult = await dynamoDBClient.PutItemAsync(new PutItemRequest
                {
                    TableName = userTableName,
                    Item = daisyControlUpdateUserDto.ToDocument(null).ToAttributeMap(),
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

                return daisyControlUpdateUserDto;
            }
            catch (ConditionalCheckFailedException)
            {
                throw new CommonException("e5f4ed44-3488-42a2-aa11-2a80405f73ca", $"Revision didn't match. User [{daisyControlUpdateUserDto.Username}] won't be updated.");
            }
            catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            }
            catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("bfc58783-5b54-4bd4-abcf-085b88f55e64", $"Unhandled exception when querying database. Failed to update User [{daisyControlUpdateUserDto.Id}] in storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> TryDeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            try
            {
                var response = await dynamoDBClient.DeleteItemAsync(new DeleteItemRequest
                {
                    TableName = userTableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "userId", new AttributeValue { S = userId } },
                    },
                }).ConfigureAwait(false);

                return response.HttpStatusCode != HttpStatusCode.NoContent;
            }
            catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            }
            catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("bcd3fe41-f87d-4434-b693-b65390a20051", $"Unhandled exception when querying database. Failed to delete User matching UserId [{userId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlStorageUserDto[]> TryGetUsersWithMessagesToProcessAsync(int limitRows = 10)
        {
            var config = CommonConfigurationManager.ReloadConfig();

            try
            {
                var queryResponse = await dynamoDBClient.QueryAsync(new QueryRequest
                {
                    TableName = userTableName,
                    Limit = limitRows,
                    ConsistentRead = false,
                    IndexName = config.StorageConfiguration.UsersWithMessagesToProcessIndexName,
                    KeyConditionExpression = "#Status = :Status AND #NbUnprocessedMessages > :NbUnprocessedMessages",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#NbUnprocessedMessages", "nbUnprocessedMessages"
                        },
                        {
                            "#Status", "status"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":NbUnprocessedMessages", new AttributeValue { N = "0" }
                        },
                        {
                            ":Status", new AttributeValue { S = UserStatus.Completed.ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlStorageUserDto> UserCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlStorageUserDto>(jsonResponse);
                    UserCollection.Add(user);
                }

                return UserCollection.ToArray();
            }
            catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            }
            catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("a82d2dbe-f49d-4888-bb4b-71a57818baf3", $"Unhandled exception when querying database to fetch users by unprocessed messages. Exception message [{ex.Message}].", ex);
            }
        }
    }
}
