using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    public class DaisyControlUserInfo
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "Unknown";

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "Unknown";
    }
}
