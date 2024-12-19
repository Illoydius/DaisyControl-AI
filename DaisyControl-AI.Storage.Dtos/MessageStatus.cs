using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageStatus
    {
        Unknown = 0,
        ToProcess = 1,
        Processed = 2,
    }
}
