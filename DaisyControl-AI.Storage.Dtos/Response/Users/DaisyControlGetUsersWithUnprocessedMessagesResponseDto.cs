using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.Response.Users
{
    public class DaisyControlGetUsersWithUnprocessedMessagesResponseDto : IStorageDto
    {
        [JsonPropertyName("users")]
        public DaisyControlUserResponseDto[] Users { get; set; }
    }
}
