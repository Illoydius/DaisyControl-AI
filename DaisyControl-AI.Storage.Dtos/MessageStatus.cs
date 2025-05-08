using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageStatus
    {
        Pending = 0,
        InProcess = 1,
        Processed = 2,
        Error = 3,
    }
}
