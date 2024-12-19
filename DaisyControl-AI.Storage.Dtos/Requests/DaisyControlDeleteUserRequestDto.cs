using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests
{
    /// <summary>
    /// Represent a request to delete an existing user from the database.
    /// </summary>
    public class DaisyControlDeleteUserRequestDto : IStorageDto
    {
        [FromRoute]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
