using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    /// <summary>
    /// Extensions to convert a model to a DB Document.
    /// </summary>
    public static class DynamoDbDocumentExtension
    {
        /// <summary>
        /// Converts the actual HistoryData to Document config instance for DynamoDB.
        /// </summary>
        /// <param name="dataItem">The instance of HistoryData.</param>
        /// <returns>The instance of Document for storing in DynamoDB.</returns>
        public static Document ToDocument(this IDataItem dataItem, JsonSerializerOptions serializerOptions = null)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            // Note that serializing directly the dataItem using Json.Text doesn't serialize child properties, which was working with newtonsoft. The following code fix that behaviour.
            string serializedValue = dataItem switch
            {
                DaisyControlUserDto daisyControlUserDto => JsonSerializer.Serialize(daisyControlUserDto, serializerOptions),
                
                _ => throw new Exception($"Type {dataItem.GetType()} is unhandled."),
            };

            var document = Document.FromJson(serializedValue);
            return document;
        }
    }
}
