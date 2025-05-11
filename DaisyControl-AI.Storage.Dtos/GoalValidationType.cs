using System.Text.Json.Serialization;

namespace DaisyControl_AI.Storage.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GoalValidationType
    {
        UserInfo,
        InferenceServerQuery
    }
}
