using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace DaisyControl_AI.Storage.Dtos.Storage
{
    /// <summary>
    /// Represent a User in the database.
    /// </summary>
    public class DaisyControlStorageUserDto : IStorageDto
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Represent the conversation between the user and the AI
        /// </summary>
        [JsonPropertyName("messagesHistory")]
        public List<DaisyControlMessage> MessagesHistory { get; set; }

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("nbUnprocessedMessages")]
        public int NbUnprocessedMessages { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }
    }
}
