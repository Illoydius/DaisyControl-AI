using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Storage.Dtos.Response.Users
{
    public class DaisyControlGetUsersResponseDto : IStorageDto
    {
        [JsonPropertyName("users")]
        public DaisyControlUserDto[] Users { get; set; }
    }
}
