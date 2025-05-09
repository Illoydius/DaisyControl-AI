using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageSourceType
    {
        DirectMessage = 0,
        Channel = 1,
    }
}
