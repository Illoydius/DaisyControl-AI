using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    public class DaisyControlMessage
    {
        [JsonPropertyName("referentialType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageReferentialType ReferentialType { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("messageStatus")]
        public MessageStatus MessageStatus { get; set; }

        [JsonPropertyName("messageContent")]
        public string MessageContent { get; set; }

        [JsonPropertyName("sourceInfo")]
        public MessageSourceInfo SourceInfo { get; set; }
    }
}
