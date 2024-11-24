using System.Text.Json.Serialization;

namespace DaisyControl_AI.Core.InferenceServer
{
    public class PromptGenerationRequestDto
    {
        [JsonPropertyName("max_context_length")]
        public int MaxContextLength { get; set; } = 1024;

        [JsonPropertyName("max_length")]
        public int MaxQueryLength { get; set; } = 512;

        [JsonPropertyName("prompt")]
        public string PromptContextContent { get; set; }

        [JsonPropertyName("quiet")]
        public bool Quiet { get; set; } = false;

        [JsonPropertyName("rep_pen")]
        public float RepetitionPenalty { get; set; } = 1.1f;

        [JsonPropertyName("rep_pen_range")]
        public int RepetitionPenaltyRange { get; set; } = 256;

        [JsonPropertyName("rep_pen_slope")]
        public float RepetitionPenaltySlope { get; set; } = 1;

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.7f;

        [JsonPropertyName("tfs")]
        public float Tfs { get; set; } = 1f;

        [JsonPropertyName("top_a")]
        public float TopA { get; set; } = 0f;

        [JsonPropertyName("top_k")]
        public float TopK { get; set; } = 100f;

        [JsonPropertyName("top_p")]
        public float TopP { get; set; } = 0.9f;

        [JsonPropertyName("typical")]
        public float Typical { get; set; } = 1f;
    }
}
