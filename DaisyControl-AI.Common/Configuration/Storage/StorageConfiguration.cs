using System.Text.Json.Serialization;

namespace DaisyControl_AI.Common.Configuration.Storage
{
    public class StorageConfiguration
    {
        [JsonPropertyName("usersWithMessagesToProcessIndexName")]
        public string UsersWithMessagesToProcessIndexName { get; set; }

        [JsonPropertyName("usersWithLastThoughtAboutTimeIndexName")]
        public string UsersWithLastThoughtAboutTimeIndexName { get; set; }

        [JsonPropertyName("usersWithInferenceTasksIndexName")]
        public string UsersWithInferenceTasksIndexName { get; set; }

        [JsonPropertyName("usersWithFollowUpAvailabilityIndexName")]
        public string UsersWithFollowUpAvailabilityIndexName { get; set; }

        [JsonPropertyName("personasWithInferenceTasksIndexName")]
        public string PersonasWithInferenceTasksIndexName { get; set; }
    }
}
