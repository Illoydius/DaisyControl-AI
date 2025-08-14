using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.AIPersona;

namespace DaisyControl_AI.Storage.Dtos.Requests.Personas
{
    /// <summary>
    /// Represent a request to update an existing Persona to the database.
    /// </summary>
    public class DaisyControlUpdatePersonaRequestDto: DaisyControlAIPersonaDto, IDataItem
    {
    }
}
