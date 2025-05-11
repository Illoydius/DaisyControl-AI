using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    public class InferenceTask
    {
        [JsonPropertyName("keyType")]
        public InferenceTaskKeyType KeyType { get; set; }

        [JsonPropertyName("keyValue")]
        public string KeyValue { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }
    }
}
