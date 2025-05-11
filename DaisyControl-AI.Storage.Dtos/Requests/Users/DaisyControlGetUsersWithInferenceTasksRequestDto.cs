using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.Users
{
    /// <summary>
    /// Represent a request to get a chunk of users with pending inference tasks.
    /// </summary>
    public class DaisyControlGetUsersWithInferenceTasksRequestDto : IStorageDto
    {
        [FromQuery]
        [JsonPropertyName("maxNbUsersToFetch")]
        public int MaxNbUsersToFetch { get; set; } = 10;
    }
}
