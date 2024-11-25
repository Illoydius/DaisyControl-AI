using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace DaisyControl_AI.Storage.Dtos
{
    public class DaisyControlMessage
    {
        //[JsonConverter(typeof(StringEnumConverter))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("referentialType")]
        public MessageReferentialType ReferentialType { get; set; }

        [JsonPropertyName("messageContent")]
        public string MessageContent { get; set; }
    }
}
