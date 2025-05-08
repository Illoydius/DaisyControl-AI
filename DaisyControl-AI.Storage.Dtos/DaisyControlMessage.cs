using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace DaisyControl_AI.Storage.Dtos
{
    public class DaisyControlMessage
    {
        [JsonPropertyName("referentialType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageReferentialType ReferentialType { get; set; }

        [JsonPropertyName("messageContent")]
        public string MessageContent { get; set; }

        //[JsonPropertyName("messageStatus")]
        //public MessageStatus MessageStatus { get; set; }

        [JsonPropertyName("messageSource")]
        public MessageSource MessageSource { get; set; }
    }
}
