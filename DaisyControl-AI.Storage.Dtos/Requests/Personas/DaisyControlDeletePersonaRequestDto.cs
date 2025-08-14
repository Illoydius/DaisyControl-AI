using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.Personas
{
    /// <summary>
    /// Represent a request to delete an existing Persona from the database.
    /// </summary>
    public class DaisyControlDeletePersonaRequestDto : IStorageDto
    {
        [FromRoute]
        [JsonPropertyName("personaId")]
        public string PersonaId { get; set; }
    }
}
