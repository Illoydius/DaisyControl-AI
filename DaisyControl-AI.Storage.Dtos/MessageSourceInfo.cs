using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    public class MessageSourceInfo
    {
        [JsonPropertyName("messageSource")]
        public MessageSource MessageSource { get; set; } 

        [JsonPropertyName("messageSourceType")]
        public MessageSourceType MessageSourceType { get; set; } 

        [JsonPropertyName("messageSourceReferential")]
        public string MessageSourceReferential { get; set; } 
    }
}
