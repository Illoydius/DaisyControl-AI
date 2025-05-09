using System.Text.Json.Serialization;

namespace DaisyControl_AI.Common.Configuration.Storage
{
    public class InferenceNodeConfiguration
    {
        [JsonPropertyName("getPendingMessagesFromBufferUrl")]
        public string GetPendingMessagesFromBufferUrl { get; set; } = $"{DaisyControlConstants.StorageWebApiBaseUrl}/api/messagesbuffer/reserve?maxNbPendingMessagesToFetch=3";

        [JsonPropertyName("inferenceServerConfiguration")]
        public InferenceServerConfiguration InferenceServerConfiguration { get; set; }
    }
}
