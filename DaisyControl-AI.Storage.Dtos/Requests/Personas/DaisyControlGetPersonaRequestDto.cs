using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Dtos.Requests.Personas
{
    /// <summary>
    /// Represent a request to add a new Persona to the database.
    /// </summary>
    public class DaisyControlGetPersonaRequestDto : IStorageDto
    {
        [FromRoute]
        [JsonPropertyName("personaId")]
        public string PersonaId { get; set; }
    }
}
