using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserStatus
    {
        Ready = 0,
        UserMessagePending = 1,
        AIMessagePending = 2,
        Working = 3,
    }
}
