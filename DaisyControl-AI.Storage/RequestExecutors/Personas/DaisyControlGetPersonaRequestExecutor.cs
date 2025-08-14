using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Personas
{
    public class DaisyControlGetPersonaRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetPersonaRequestDto daisyControlGetPersonaDto = null;
        private IPersonasDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetPersonaRequestExecutor(
            IPersonasDal daisyControlDal,
            DaisyControlGetPersonaRequestDto daisyControlGetPersonaDto)
        {
            this.daisyControlGetPersonaDto = daisyControlGetPersonaDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlGetPersonaDto?.PersonaId == null)
            {
                response = "Invalid Dto. Request payload was incorrect.";
                return false;
            }

            // Get Persona from storage to check if it already exists
            var user = await daisyControlDal.TryGetPersonaAsync(daisyControlGetPersonaDto.PersonaId);

            response = user;//PersonaDtoConverter.ConvertStoragePersonaToGetResponseDto(user);
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
