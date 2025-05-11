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

        [JsonPropertyName("email")]
        public string Email { get; set; } = null;

        [JsonPropertyName("age")]
        public int? Age { get; set; }

        [JsonPropertyName("gender")]
        public Gender? Gender { get; set; }

        [JsonPropertyName("genitals")]
        public Genitals? Genitals { get; set; }

        [JsonPropertyName("location")]
        public UserLocation Location { get; set; } = new();

        [JsonPropertyName("workOccupation")]
        public UserWorkOccupation WorkOccupation { get; set; } = new();

        public int GetFamiliarityPercentage()
        {
            int familiarity = 0;

            if (this.FirstName.ToLowerInvariant() != "unknown")
            {
                familiarity += 2;
            }

            if (this.LastName.ToLowerInvariant() != "unknown")
            {
                familiarity += 3;
            }

            if (this.Genitals != null)
            {
                 familiarity += 5;
            }

            if (this.Age != null)
            {
                familiarity += 5;
            }

            if (!string.IsNullOrWhiteSpace(this.Email))
            {
                familiarity += 2;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.WorkTitle))
            {
                familiarity += 5;
            }

            if (this.WorkOccupation.AnnualSalary != null)
            {
                familiarity += 3;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.Company.Name))
            {
                familiarity += 3;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.Company.Address))
            {
                familiarity += 5;
            }

            // TODO: if AI was in past events with User, add familiarity

            return familiarity;
        }
    }
}
