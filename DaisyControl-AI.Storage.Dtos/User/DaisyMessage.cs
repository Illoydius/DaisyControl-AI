using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class DaisyMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("thoughts")]
        public string Thoughts { get; set; }
    }
}
