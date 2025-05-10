using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class DaisyControlUserInfo
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "Unknown";

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "Unknown";

        [JsonPropertyName("age")]
        public int? Age { get; set; }

        [JsonPropertyName("genitals")]
        public Genitals? Genitals { get; set; }

        [JsonPropertyName("location")]
        public UserLocation Location { get; set; } = new();

        public int GetFamiliarityPercentage()
        {
            int familiarity = 0;
            if (this.FirstName.ToLowerInvariant() == "unknown")
            {
                familiarity += 2;
            }

            if (this.LastName.ToLowerInvariant() == "unknown")
            {
                familiarity += 3;
            }

            if (this.Genitals == null)
            {
                 familiarity += 5;
            }

            if (this.Age == null)
            {
                familiarity += 5;
            }

            // TODO: if AI was in past events with User, add familiarity

            return familiarity;
        }
    }
}
