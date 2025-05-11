using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos.User
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
    }
}
