using System.Text.Json.Serialization;

namespace DaisyControl_AI.WebAPI.Dtos
{
    public class MainPostSendMessageToAIDto : IDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
