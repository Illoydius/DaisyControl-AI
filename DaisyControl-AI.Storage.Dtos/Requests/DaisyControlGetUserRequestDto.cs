using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests
{
    /// <summary>
    /// Represent a request to add a new user to the database.
    /// </summary>
    public class DaisyControlGetUserRequestDto : IStorageDto
    {
        [FromRoute]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
