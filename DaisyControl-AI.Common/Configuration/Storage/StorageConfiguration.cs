using System.Text.Json.Serialization;

namespace DaisyControl_AI.Common.Configuration.Storage
{
    public class StorageConfiguration
    {
        [JsonPropertyName("usersWithMessagesToProcessIndexName")]
        public string UsersWithMessagesToProcessIndexName { get; set; }
    }
}
