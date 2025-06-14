using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class UserLocation : IUserInfoData
    {
        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }

        [JsonPropertyName("provinceOrStateOrDistrict")]
        public string ProvinceOrStateOrDistrict { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("civicAddress")]
        public string CivicAddress { get; set; }

        // TODO: Latitude longitude? Yeah maybe a bit much? Could give us info about timezone, weather, close by other users, etc? Would that be better than what we have now?...
    }
}
