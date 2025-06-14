using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class UserSexuality : IUserInfoData
    {
        [JsonPropertyName("maritalStatus")]
        public MaritalStatus? MaritalStatus { get; set; }

        [JsonPropertyName("sexualPartner")]
        public SexualPartner SexualPartner { get; set; } = new();
    }
}
