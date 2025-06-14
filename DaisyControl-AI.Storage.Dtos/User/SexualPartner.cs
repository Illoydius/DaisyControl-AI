using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    /// <summary>
    /// It's basically a stripped down version of the DaisyControlUserInfo as we don't need that much info about the user's partner
    /// </summary>
    public class SexualPartner
    {
        [JsonPropertyName("sexualPartnerFirstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("sexualPartnerLastName")]
        public string LastName { get; set; }

        [JsonPropertyName("sexualPartnerGender")]
        public Gender? Gender { get; set; }
    }
}
