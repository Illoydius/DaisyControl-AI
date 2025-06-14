using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MaritalStatus
    {
        Single,
        Couple,
        Married,
    }
}
