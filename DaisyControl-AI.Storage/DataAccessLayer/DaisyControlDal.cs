using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.Exceptions;
using DaisyControl_AI.Storage.Dtos.Date;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Response.Users;

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
            CleanUpUsersTable().Wait();
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

                    // Users Table
                    if (!tables.TableNames.Contains(userTableName))
                    {
                        await CreateTable(new CreateTableRequest(userTableName, new List<KeySchemaElement>
                        {
                            new KeySchemaElement("userId", KeyType.HASH),
                            //new KeySchemaElement("status", KeyType.RANGE),
                        }, new List<AttributeDefinition>
                        {
                            new AttributeDefinition("userId", ScalarAttributeType.S),
                            new AttributeDefinition("status", ScalarAttributeType.S),
                            new AttributeDefinition("nextMessageToProcessOperationAvailabilityAtUtc", ScalarAttributeType.N),
                            new AttributeDefinition("nextImmediateGoalOperationAvailabilityAtUtc", ScalarAttributeType.N),
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
                                    IndexName = config.StorageConfiguration.UsersWithMessagesToProcessIndexName,
                                    KeySchema = new List<KeySchemaElement>
                                    {
                                        new("status", KeyType.HASH),
                                        new("nextMessageToProcessOperationAvailabilityAtUtc", KeyType.RANGE),
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
                                new()
                                {
                                    IndexName = config.StorageConfiguration.UsersWithOldestImmediateGoalsTimeIndexName,
                                    KeySchema = new List<KeySchemaElement>
                                    {
                                        new("status", KeyType.HASH),
                                        new("nextImmediateGoalOperationAvailabilityAtUtc", KeyType.RANGE),
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
                                new()
                                {
                                    IndexName = config.StorageConfiguration.UsersWithInferenceTasksIndexName,
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

        private async Task CleanUpUsersTable()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    cancellationTokenSource.CancelAfter(NbMsUnreachableDbOnStartup);
                    ListTablesResponse tables = await dynamoDBClient.ListTablesAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                    if (tables.TableNames.Contains(userTableName))
                    {
                        LoggingManager.LogToFile("0b16fd3b-c368-49c2-846f-d08207d6f93d", $"Cleaning up Users table...", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                        // Cleanup users stuck with Working status
                        var stuckUsers = await TryGetUsersWithWorkingStatusAsync(1000);

                        if (stuckUsers != null)
                        {
                            var now = DateTime.UtcNow;
                            foreach (var user in stuckUsers.Users.Where(w => (now - w.LastModifiedAtUtc).TotalMilliseconds >= 15000))
                            {
                                user.Status = UserStatus.Ready;
                                user.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;
                                var json = JsonSerializer.Serialize(user);
                                var userAsDto = JsonSerializer.Deserialize<DaisyControlUpdateUserRequestDto>(json);
                                await TryUpdateUserAsync(userAsDto);
                            }
                        }
                    }

                } catch (Exception e)
                {
                    LoggingManager.LogToFile("864b9c16-44a9-455c-a1eb-b6f752702b4b", $"Error. Couldn't clean up Users table when Initializing Database!");
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
        public async Task<DaisyControlGetUserResponseDto> TryGetUserAsync(string userId)
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
                var userDto = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);

                return userDto;

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
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
            daisyControlAddUserDto.Status = UserStatus.Ready;
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
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("fb23d483-9131-45f8-90f4-3bd192bfe520", $"User was created by another instance. User [{daisyControlAddUserDto.UserInfo.Username}] won't be created to avoid duplicates.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("008d82d9-4181-4466-be67-07533b912df0", $"Unhandled exception when querying database. Failed to add User of Name [{daisyControlAddUserDto.UserInfo.Username}] to storage. Exception message [{ex.Message}].", ex);
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

            // If we have a pending message, set the User status to Pending
            if (daisyControlUpdateUserDto.Status != UserStatus.Working)
            {
                if (daisyControlUpdateUserDto.Status != UserStatus.AIMessagePending && daisyControlUpdateUserDto.MessagesHistory.Any(a => a.ReferentialType == MessageReferentialType.Assistant && a.MessageStatus == MessageStatus.Pending))
                {
                    daisyControlUpdateUserDto.Status = UserStatus.AIMessagePending;
                } else if (daisyControlUpdateUserDto.Status != UserStatus.UserMessagePending && daisyControlUpdateUserDto.MessagesHistory.Any(a => a.ReferentialType == MessageReferentialType.User && a.MessageStatus == MessageStatus.Pending))
                {
                    daisyControlUpdateUserDto.Status = UserStatus.UserMessagePending;
                } else if (daisyControlUpdateUserDto.Status == UserStatus.AIMessagePending && !daisyControlUpdateUserDto.MessagesHistory.Any(a => a.ReferentialType == MessageReferentialType.Assistant && a.MessageStatus == MessageStatus.Pending) ||
                           daisyControlUpdateUserDto.Status == UserStatus.UserMessagePending && !daisyControlUpdateUserDto.MessagesHistory.Any(a => a.ReferentialType == MessageReferentialType.User && a.MessageStatus == MessageStatus.Pending))
                {
                    daisyControlUpdateUserDto.Status = UserStatus.Ready;
                }
            }

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
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("e5f4ed44-3488-42a2-aa11-2a80405f73ca", $"Revision didn't match. User [{daisyControlUpdateUserDto.UserInfo.Username}] won't be updated.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
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
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("bcd3fe41-f87d-4434-b693-b65390a20051", $"Unhandled exception when querying database. Failed to delete User matching UserId [{userId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetUsersResponseDto> TryGetUsersWithUserMessagesToProcessAsync(int limitRows)
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
                    KeyConditionExpression = "#status = :status AND #nextMessageToProcessOperationAvailabilityAtUtc < :nextMessageToProcessOperationAvailabilityAtUtc",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                        {
                            "#nextMessageToProcessOperationAvailabilityAtUtc", "nextMessageToProcessOperationAvailabilityAtUtc"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.UserMessagePending.ToString() }
                        },
                        {
                            ":nextMessageToProcessOperationAvailabilityAtUtc", new AttributeValue { N = DateTime.UtcNow.ToUnixTime().ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetUserResponseDto> UsersCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);
                    UsersCollection.Add(user);
                }

                return new DaisyControlGetUsersResponseDto
                {
                    Users = UsersCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("0333ac3c-5f3b-49a7-a34d-763c500bd691", $"Unhandled exception when querying database to fetch users with pending User messages. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetUsersResponseDto> TryGetUsersWithAIMessagesToProcessAsync(int limitRows)
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
                    KeyConditionExpression = "#status = :status AND #nextMessageToProcessOperationAvailabilityAtUtc < :nextMessageToProcessOperationAvailabilityAtUtc",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                        {
                            "#nextMessageToProcessOperationAvailabilityAtUtc", "nextMessageToProcessOperationAvailabilityAtUtc"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.AIMessagePending.ToString() }
                        },
                        {
                            ":nextMessageToProcessOperationAvailabilityAtUtc", new AttributeValue { N = DateTime.UtcNow.ToUnixTime().ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetUserResponseDto> UsersCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);
                    UsersCollection.Add(user);
                }

                return new DaisyControlGetUsersResponseDto
                {
                    Users = UsersCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("49e068e8-920c-4861-9e95-0f975146f530", $"Unhandled exception when querying database to fetch users with pending AI messages. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetUsersResponseDto> TryGetUsersWithWorkingStatusAsync(int limitRows)
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
                    KeyConditionExpression = "#status = :status AND #nextMessageToProcessOperationAvailabilityAtUtc < :nextMessageToProcessOperationAvailabilityAtUtc",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                        {
                            "#nextMessageToProcessOperationAvailabilityAtUtc", "nextMessageToProcessOperationAvailabilityAtUtc"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.Working.ToString() }
                        },
                        {
                            ":nextMessageToProcessOperationAvailabilityAtUtc", new AttributeValue { N = DateTime.UtcNow.AddYears(10).ToUnixTime().ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetUserResponseDto> UsersCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);
                    UsersCollection.Add(user);
                }

                return new DaisyControlGetUsersResponseDto
                {
                    Users = UsersCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("cbb7ec69-a9a5-4079-b798-a37c31512aa1", $"Unhandled exception when querying database to fetch users with working status. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetUsersResponseDto> TryGetUsersWithOldestImmediateGoalsRefreshTimeAsync(int limitRows)
        {
            var config = CommonConfigurationManager.ReloadConfig();

            try
            {
                var queryResponse = await dynamoDBClient.QueryAsync(new QueryRequest
                {
                    TableName = userTableName,
                    Limit = limitRows,
                    ConsistentRead = false,
                    IndexName = config.StorageConfiguration.UsersWithOldestImmediateGoalsTimeIndexName,
                    KeyConditionExpression = "#status = :status AND #nextImmediateGoalOperationAvailabilityAtUtc < :nextImmediateGoalOperationAvailabilityAtUtc",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                        {
                            "#nextImmediateGoalOperationAvailabilityAtUtc", "nextImmediateGoalOperationAvailabilityAtUtc"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.Ready.ToString() }
                        },
                        {
                            ":nextImmediateGoalOperationAvailabilityAtUtc", new AttributeValue { N = DateTime.UtcNow.ToUnixTime().ToString() }
                        },
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetUserResponseDto> UsersCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);
                    UsersCollection.Add(user);
                }

                return new DaisyControlGetUsersResponseDto
                {
                    Users = UsersCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("cbb7ec69-a9a5-4079-b798-a37c31512aa1", $"Unhandled exception when querying database to fetch users with working status. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlGetUsersResponseDto> TryGetUsersWithPendingInferenceTasksAsync(int limitRows)
        {
            var config = CommonConfigurationManager.ReloadConfig();

            try
            {
                var queryResponse = await dynamoDBClient.QueryAsync(new QueryRequest
                {
                    TableName = userTableName,
                    Limit = limitRows,
                    ConsistentRead = false,
                    IndexName = config.StorageConfiguration.UsersWithInferenceTasksIndexName,
                    KeyConditionExpression = "#status = :status AND #pendingInferenceTasksCounter > :minInferenceTasksCounter",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {
                            "#status", "status"
                        },
                        {
                            "#pendingInferenceTasksCounter", "pendingInferenceTasksCounter"
                        },
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {
                            ":status", new AttributeValue { S = UserStatus.Ready.ToString() }
                        },
                        {
                            ":minInferenceTasksCounter", new AttributeValue { N = "0" }
                        }
                    },
                }).ConfigureAwait(false);

                if (queryResponse.Items.Count <= 0)
                {
                    return null; // no Items found
                }

                List<DaisyControlGetUserResponseDto> UsersCollection = new();

                foreach (Dictionary<string, AttributeValue> itemFromDatabase in queryResponse.Items)
                {
                    var responseDocument = Document.FromAttributeMap(itemFromDatabase);
                    var jsonResponse = responseDocument.ToJson();
                    var user = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(jsonResponse);
                    UsersCollection.Add(user);
                }

                return new DaisyControlGetUsersResponseDto
                {
                    Users = UsersCollection.ToArray(),
                };

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // wrap exception
                throw new CommonException("fbaabaae-1df2-4027-8fab-fa442e89f5f7", $"Unhandled exception when querying database to fetch users with pending inference tasks. Exception message [{ex.Message}].", ex);
            }
        }
    }
}
