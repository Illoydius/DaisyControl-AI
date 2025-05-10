using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.Users
{
    /// <summary>
    /// Represent a request to get a chunk of users with the oldest immediate goals refresh time.
    /// </summary>
    public class DaisyControlGetUsersWithOldestImmediateGoalsRefreshTimeRequestDto : IStorageDto
    {
        [FromQuery]
        [JsonPropertyName("maxNbUsersToFetch")]
        public int MaxNbUsersToFetch { get; set; } = 10;
    }
}
