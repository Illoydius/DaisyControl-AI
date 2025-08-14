using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Response.Personas;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public interface IPersonasDal
    {
        // Personas
        Task<DaisyControlGetPersonaResponseDto> TryGetPersonaAsync(string userId);
        Task<DaisyControlAddPersonaRequestDto> TryAddPersonaAsync(DaisyControlAddPersonaRequestDto daisyControlAddPersonaDto);
        Task<DaisyControlUpdatePersonaRequestDto> TryUpdatePersonaAsync(DaisyControlUpdatePersonaRequestDto daisyControlUpdatePersonaDto);
        Task<bool> TryDeletePersonaAsync(string userId);
    }
}
