using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.AIPersona;

namespace DaisyControl_AI.Storage.Dtos.Requests.Personas
{
    /// <summary>
    /// Represent a request to add a new Persona to the database.
    /// </summary>
    public class DaisyControlAddPersonaRequestDto : DaisyControlAIPersonaDto, IDataItem
    {
    }
}
