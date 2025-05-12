using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class UserLocation
    {
        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }
    }
}
