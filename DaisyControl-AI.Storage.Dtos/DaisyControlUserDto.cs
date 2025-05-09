using System.Text.Json.Serialization;
using DaisyControl_AI.Core.DaisyMind.DaisyMemory.AI;
using DaisyControl_AI.Storage.Dtos.JsonConverters;

namespace DaisyControl_AI.Storage.Dtos
{
    public class DaisyControlUserDto : IStorageDto
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("userInfo")]
        public DaisyControlUserInfo UserInfo { get; set; }

        [JsonPropertyName("messagesHistory")]
        public List<DaisyControlMessage> MessagesHistory { get; set; }

        [JsonPropertyName("aiGlobal")]
        public DaisyControlStorageMind AIGlobal { get; set; } = new();

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }

        [JsonConverter(typeof(DateTimeUnixJsonConverter))]
        [JsonPropertyName("nextOperationAvailabilityAtUtc")]
        public DateTime NextOperationAvailabilityAtUtc {get; set; }
    }
}
