using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InferenceTaskKeyType
    {
        GoalValidation = 0,
    }
}
