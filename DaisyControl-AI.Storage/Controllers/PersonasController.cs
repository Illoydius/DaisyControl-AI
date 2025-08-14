using DaisyControl_AI.Common.Exceptions.HTTP;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : Controller
    {
        private IPersonasWorkflow workflow;

        public PersonasController(IPersonasWorkflow workflow)
        {
            this.workflow = workflow;
        }

        /// <summary>
        /// Get new Persona from storage.
        /// </summary>
        [HttpGet]
        [Route("{personaId}")]
        public async Task<ActionResult<object>> GetPersona(DaisyControlGetPersonaRequestDto personaRequest)
        {
            return await workflow.ExecuteAsync(personaRequest);
        }

        /// <summary>
        /// Add new Persona to storage.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> AddPersona([FromBody] DaisyControlAddPersonaRequestDto personaRequest)
        {
            return await workflow.ExecuteAsync(personaRequest);
        }

        /// <summary>
        /// Update (full obj) Persona to storage.
        /// </summary>
        [HttpPut]
        [Route("{personaIdToUpdate}")]
        public async Task<ActionResult<object>> UpdatePersona([FromRoute] string personaIdToUpdate, DaisyControlUpdatePersonaRequestDto personaRequest)
        {
            // Validate request
            if (personaRequest.Id != personaIdToUpdate)
            {
                throw new BadRequestWebApiException("98e0f9e2-0b43-4640-8433-a75785bebc76", $"PersonaId [{personaIdToUpdate}] to update didn't match the provided body PersonaId [{personaRequest.Id}].");
            }

            return await workflow.ExecuteAsync(personaRequest);
        }

        /// <summary>
        /// Delete Persona from storage.
        /// </summary>
        [HttpDelete]
        [Route("{personaId}")]
        public async Task<ActionResult<object>> DeletePersona(DaisyControlDeletePersonaRequestDto personaRequest)
        {
            return await workflow.ExecuteAsync(personaRequest);
        }
    }
}
