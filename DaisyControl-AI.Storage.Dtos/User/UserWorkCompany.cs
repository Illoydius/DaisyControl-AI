using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class UserWorkCompany
    {
        [JsonPropertyName("companyName")]
        public string Name { get; set; }

        [JsonPropertyName("companyAddress")]
        public string Address { get; set; }
    }
}
