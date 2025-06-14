using System.Reflection;
using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class DaisyControlUserInfo : IUserInfoData
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } // Discord username

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "Unknown";

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "Unknown";

        [JsonPropertyName("email")]
        public string Email { get; set; } = null;

        [JsonPropertyName("age")]
        public int? Age { get; set; }

        [JsonPropertyName("gender")]
        public Gender? Gender { get; set; }

        [JsonPropertyName("genitals")]
        public Genitals? Genitals { get; set; }

        [JsonPropertyName("locationCategory")]
        public UserLocation LocationCategory { get; set; } = new();

        [JsonPropertyName("workOccupationCategory")]
        public UserWorkOccupation WorkOccupationCategory { get; set; } = new();

        [JsonPropertyName("sexualCategory")]
        public UserSexuality SexualCategory { get; set; } = new();

        private int GetFamiliarityPercentageFromReflection(IUserInfoData dataType)
        {
            if (dataType == null)
            {
                return 0;
            }

            var properties = dataType.GetType().GetProperties();

            int familiarity = 0;
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType?.GetInterfaces()?.Any(a => a == typeof(IUserInfoData)) == true)
                {
                    var data = (IUserInfoData)property.GetValue(dataType);
                    familiarity += GetFamiliarityPercentageFromReflection(data);
                    continue;
                }

                familiarity += GetFamiliarityPercentageOfSpecificProperty(property, dataType);
            }

            return familiarity;
        }

        private int GetFamiliarityPercentageOfSpecificProperty(PropertyInfo propertyInfo, IUserInfoData dataType)
        {
            if (propertyInfo == null)
            {
                return 0;
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                var strValue = propertyInfo.GetValue(dataType) as string;
                if (!string.IsNullOrWhiteSpace(strValue) && InvalidPropertiesValues().All(a => a != strValue.ToLowerInvariant()))
                {
                    return 1;
                }
            } else
            {
                if (propertyInfo.GetValue(dataType) != null)
                {
                    return 1;
                }
            }

            return 0;
        }

        public int GetFamiliarityPercentage()
        {
            return GetFamiliarityPercentageFromReflection(this);
        }

        public static string[] InvalidPropertiesValues()
        {
            return
            [
                "unknown",
                "null",
                "empty",
                "not found",
                "undefined",
            ];
        }
    }
}
