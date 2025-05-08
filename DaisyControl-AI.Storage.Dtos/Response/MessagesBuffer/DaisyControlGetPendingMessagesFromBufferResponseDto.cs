using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.Response.Messages;

namespace DaisyControl_AI.Storage.Dtos.Response
{
    public class DaisyControlGetPendingMessagesFromBufferResponseDto : IStorageDto
    {
        [JsonPropertyName("messages")]
        public DaisyControlMessageToBufferResponseDto[] Messages { get; set; }
    }
}
