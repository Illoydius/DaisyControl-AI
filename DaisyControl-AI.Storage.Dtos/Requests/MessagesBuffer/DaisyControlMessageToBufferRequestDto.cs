using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.Response.Messages
{
    public class DaisyControlMessageToBufferRequestDto : IStorageDto
    {
        [JsonPropertyName("messageId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("status")]
        public MessageStatus Status { get; set; }

        [JsonPropertyName("message")]
        public DaisyControlMessage Message { get; set; }
    }
}
