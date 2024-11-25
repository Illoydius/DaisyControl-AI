using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.Exceptions;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public class DaisyControlDal : IDaisyControlDal
    {
        private const string AssumeRoleAwsExceptionErrorMessage = "Error calling AssumeRole for role";
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
                        }, new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 1000,
                            WriteCapacityUnits = 1000,
                        }));
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

            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // rewrite exception
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
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("fb23d483-9131-45f8-90f4-3bd192bfe520", $"User was created by another instance. User [{daisyControlAddUserDto.Username}] won't be created to avoid duplicates.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // rewrite exception
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
            } catch (ConditionalCheckFailedException)
            {
                throw new CommonException("e5f4ed44-3488-42a2-aa11-2a80405f73ca", $"Revision didn't match. User [{daisyControlUpdateUserDto.Username}] won't be updated.");
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(NbMsToDelayAfterProvisionException);

                throw;
            } catch (Exception ex)
            {
                // rewrite exception
                throw new CommonException("bfc58783-5b54-4bd4-abcf-085b88f55e64", $"Unhandled exception when querying database. Failed to update User [{daisyControlUpdateUserDto.Id}] to storage. Exception message [{ex.Message}].", ex);
            }
        }
    }
}
