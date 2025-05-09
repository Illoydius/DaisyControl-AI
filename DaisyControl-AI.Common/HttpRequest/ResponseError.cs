using System.Text.Json.Serialization;

namespace DaisyControl_AI.Common.HttpRequest
{
    public class ResponseError
    {
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("errorCode")]
        public long ErrorCode { get; set; } = -1;

        public bool ValidErrorCode() => this.ErrorCode >= 0;
    }
}
