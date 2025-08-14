using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.AIPersona;

namespace DaisyControl_AI.Storage.Dtos.Response.Personas
{
    public class DaisyControlGetPersonasResponseDto : IStorageDto
    {
        [JsonPropertyName("personas")]
        public DaisyControlAIPersonaDto[] Personas { get; set; }
    }
}
