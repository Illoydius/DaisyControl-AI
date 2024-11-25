using System.Text.Json.Serialization;
using DaisyControl_AI.Core.DaisyMind.DaisyMemory.AI;

namespace DaisyControl_AI.Storage.Dtos.Response
{
    public class DaisyControlUserResponseDto : IStorageDto
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "Unknown";

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "Unknown";

        [JsonPropertyName("messagesHistory")]
        public List<DaisyControlMessage> MessagesHistory { get; set; }

        [JsonPropertyName("aiGlobal")]
        public DaisyControlStorageMind AIGlobal { get; set; } = new();

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}
