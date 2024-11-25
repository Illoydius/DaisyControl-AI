using System.Text.Json.Serialization;

namespace DaisyControl_AI.Core.InferenceServer
{
    public class InferenceServerPromptResultResponseDto
    {
        [JsonPropertyName("text")]
        public string Text {get;set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason {get;set; }

        [JsonPropertyName("logprobs")]
        public string Logprobs {get;set; }

        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens {get;set; }

        [JsonPropertyName("completion_tokens")]
        public int MaxTokens {get;set; }
    }
}
