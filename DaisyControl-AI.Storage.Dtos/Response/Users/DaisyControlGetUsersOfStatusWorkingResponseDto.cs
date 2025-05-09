using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.Response.Users
{
    public class DaisyControlGetUsersOfStatusWorkingResponseDto : IStorageDto
    {
        [JsonPropertyName("users")]
        public DaisyControlUserDto[] Users { get; set; }
    }
}
