using System.Text.Json.Serialization;

namespace DaisyControl_AI.Core.DaisyMind.DaisyMemory.AI
{
    public class DaisyControlStorageMind
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "Daisy";

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "Bennet";
    }
}
