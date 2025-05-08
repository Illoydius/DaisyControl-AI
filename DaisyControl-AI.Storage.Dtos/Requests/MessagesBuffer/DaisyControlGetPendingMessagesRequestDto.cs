using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.Users
{
    /// <summary>
    /// Represent a request to get a chunk of pending message requests to process.
    /// </summary>
    public class DaisyControlGetPendingMessagesRequestDto : IStorageDto
    {
        [FromQuery]
        [JsonPropertyName("maxNbPendingMessagesToFetch")]
        public int MaxNbPendingMessagesToFetch { get; set; } = 3;
    }
}
