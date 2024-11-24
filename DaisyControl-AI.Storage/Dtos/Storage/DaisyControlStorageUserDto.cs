using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.Storage
{
    /// <summary>
    /// Represent a User in the database.
    /// </summary>
    public class DaisyControlStorageUserDto : IDto
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
