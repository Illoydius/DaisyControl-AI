using System.Text.Json.Serialization;

namespace DaisyControl_AI.InferenceNode.Executors
{
    public class ExecutorQueryResultInt
    {
        [JsonPropertyName("value")]
        public int? Value { get; set; }
    }
}
