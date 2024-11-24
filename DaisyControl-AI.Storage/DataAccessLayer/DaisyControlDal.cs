using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DaisyControl_AI.Common.Exceptions;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public class DaisyControlDal : IDaisyControlDal
    {
        private const string AssumeRoleAwsExceptionErrorMessage = "Error calling AssumeRole for role";
        private string userTableName = "DaisyControl-Users";
        private AWSCredentials AWSCredentials = new BasicAWSCredentials("local", "local");
        private string DynamoDBUri = "http://127.0.0.1:8000";
        private IAmazonDynamoDB dynamoDBClient = null;

        public DaisyControlDal()
        {
            dynamoDBClient = InitClient();
            InitDatabase().Wait();
        }

        private async Task InitDatabase()
        {
            ListTablesResponse tables = await dynamoDBClient.ListTablesAsync().ConfigureAwait(false);

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

                var responseDocument = Document.FromAttributeMap(userItemsResult.Item); ;

                var jsonResponse = responseDocument.ToJson();
                var userDto = JsonSerializer.Deserialize<DaisyControlStorageUserDto>(jsonResponse);

                return userDto;

            } catch (Exception ex)
            {
                // rewrite exception
                throw new CommonException("3a7d0b58-4c83-425a-8b88-32d33f708188", $"Unhandled exception when querying database. Failed to get User matching UserId [{userId}] from storage. Exception message [{ex.Message}].", ex);
            }
        }

        /// <inheritdoc />
        public async Task<DaisyControlAddUserDto> TryAddUserAsync(DaisyControlAddUserDto daisyControlAddUserDto)
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
                var a = daisyControlAddUserDto.ToDocument(null).ToAttributeMap();
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
                throw new CommonException("fb23d483-9131-45f8-90f4-3bd192bfe520", $"User was created by another instance. User [{daisyControlAddUserDto.Name}] won't be created to avoid duplicates.");
            } catch (AmazonClientException amazonClientException) when (amazonClientException.Message.Contains(AssumeRoleAwsExceptionErrorMessage, StringComparison.OrdinalIgnoreCase))
            {
                // This is a flaky error, retrying without change fix most of the occurrence
                await Task.Delay(3000);
                throw;
            } catch (ProvisionedThroughputExceededException)
            {
                await Task.Delay(5000);

                throw;
            } catch (Exception ex)
            {
                // rewrite exception
                throw new CommonException("008d82d9-4181-4466-be67-07533b912df0", $"Unhandled exception when querying database. Failed to add User or Name [{daisyControlAddUserDto.Name}] to storage. Exception message [{ex.Message}].", ex);
            }
        }
    }
}
