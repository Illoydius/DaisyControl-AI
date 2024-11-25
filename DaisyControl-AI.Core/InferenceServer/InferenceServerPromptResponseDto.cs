using System.Text.Json.Serialization;

namespace DaisyControl_AI.Core.InferenceServer
{
    public class InferenceServerPromptResponseDto
    {
        [JsonPropertyName("results")]
        public InferenceServerPromptResultResponseDto[] Results { get; set; }
    }
}
