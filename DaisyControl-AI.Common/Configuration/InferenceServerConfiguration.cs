namespace DaisyControl_AI.Common.Configuration
{
    public class InferenceServerConfiguration
    {
        public int MaxContextLength { get; set; }
        public int MaxTokensToGenerateInSingleQueryLength { get; set; }
        public string UrlGeneratePrompt { get; set; }
    }
}
