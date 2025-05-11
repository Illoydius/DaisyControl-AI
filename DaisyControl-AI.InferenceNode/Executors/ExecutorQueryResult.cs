using System.Text.Json.Serialization;

namespace DaisyControl_AI.InferenceNode.Executors
{
    public class ExecutorQueryResult
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
