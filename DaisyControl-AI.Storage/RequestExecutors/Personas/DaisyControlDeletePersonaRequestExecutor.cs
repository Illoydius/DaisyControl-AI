using DaisyControl_AI.Common.Exceptions.HTTP;
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Personas
{
    public class DaisyControlDeletePersonaRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlDeletePersonaRequestDto daisyControlDeletePersonaDto = null;
        private IPersonasDal daisyControlDal = null;
        private object response = null;

        public DaisyControlDeletePersonaRequestExecutor(
            IPersonasDal daisyControlDal,
            DaisyControlDeletePersonaRequestDto daisyControlDeletePersonaDto)
        {
            this.daisyControlDeletePersonaDto = daisyControlDeletePersonaDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlDeletePersonaDto?.PersonaId == null)
            {
                response = "Invalid Dto. Request payload was incorrect.";
                return false;
            }

            // Get Persona from storage to check if it exists
            var user = await daisyControlDal.TryGetPersonaAsync(daisyControlDeletePersonaDto.PersonaId);

            if(user == null)
            {
                throw new BadRequestWebApiException("cd0902b5-c57a-4200-9682-edf1ebe1c87d", $"PersonaId [{daisyControlDeletePersonaDto.PersonaId}] to delete didn't exist in the storage.");
            }

            bool result = await daisyControlDal.TryDeletePersonaAsync(daisyControlDeletePersonaDto.PersonaId);
            response = result;
            return result;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
