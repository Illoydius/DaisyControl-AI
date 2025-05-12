using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class DaisyControlUserInfo
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

        [JsonPropertyName("location")]
        public UserLocation Location { get; set; } = new();

        [JsonPropertyName("workOccupation")]
        public UserWorkOccupation WorkOccupation { get; set; } = new();

        public int GetFamiliarityPercentage()
        {
            int familiarity = 0;

            if (this.FirstName.ToLowerInvariant() != "unknown")
            {
                familiarity++;
            }

            if (this.LastName.ToLowerInvariant() != "unknown")
            {
                familiarity++;
            }

            if (!string.IsNullOrWhiteSpace(this.Email))
            {
                familiarity++;
            }

            if (this.Age != null)
            {
                familiarity++;
            }

            if (this.Gender != null)
            {
                 familiarity++;
            }

            if (this.Genitals != null)
            {
                 familiarity++;
            }

            // Location
            if (!string.IsNullOrWhiteSpace(this.Location.CountryName))
            {
                familiarity++;
            }

            // WorkOccupation
            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.WorkTitle))
            {
                familiarity++;
            }

            if (this.WorkOccupation.AnnualSalary != null)
            {
                familiarity++;
            }

            // Company
            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.Company.Name))
            {
                familiarity++;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.Company.Address))
            {
                familiarity++;
            }

            if (!string.IsNullOrWhiteSpace(this.WorkOccupation.WorkDescriptionSummary))
            {
                familiarity++;
            }

            // TODO: if AI was in past events with User, add familiarity

            return familiarity;
        }
    }
}
