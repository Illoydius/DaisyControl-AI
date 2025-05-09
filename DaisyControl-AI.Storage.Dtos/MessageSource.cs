using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageSource
    {
        // TODO: Add emails + others
        Unknown = 0,
        Inferenced = 1,
        Discord = 2,
    }
}
