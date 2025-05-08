using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.MessagesBuffer
{
    public class DaisyControlGetMessageFromBufferRequestDto : IStorageDto
    {
        [FromRoute]
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
    }
}
