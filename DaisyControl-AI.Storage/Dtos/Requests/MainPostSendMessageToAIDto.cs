using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.Requests
{
    public class MainPostSendMessageToAIDto : IDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
