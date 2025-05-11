using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class UserWorkOccupation
    {
        [JsonPropertyName("workTitle")]
        public string WorkTitle { get; set; }

        [JsonPropertyName("annualSalary")]
        public int? AnnualSalary { get; set; }

        [JsonPropertyName("company")]
        public UserWorkCompany Company { get; set; } = new();

        /// <summary>
        /// A small summary of what the work entails
        /// </summary>
        [JsonPropertyName("workDescriptionSummary")]
        public string WorkDescriptionSummary { get; set; }
    }
}
