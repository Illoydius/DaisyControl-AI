using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    /// <summary>
    /// Represent a goal. This could be to know an information about the user, get a promotion at work, buy a new coffee machine, whatever.
    /// </summary>
    public class DaisyGoal
    {
         [JsonPropertyName("promptInjectionGoal")]
        public string PromptInjectionGoal { get; set; }
    }
}
