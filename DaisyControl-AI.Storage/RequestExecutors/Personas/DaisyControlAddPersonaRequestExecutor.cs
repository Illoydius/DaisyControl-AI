using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Personas
{
    public class DaisyControlAddPersonaRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddPersonaRequestDto daisyControlAddPersonaDto = null;
        private IPersonasDal daisyControlDal = null;
        private object response = null;

        public DaisyControlAddPersonaRequestExecutor(
            IPersonasDal daisyControlDal,
            DaisyControlAddPersonaRequestDto daisyControlAddPersonaDto)
        {
            this.daisyControlAddPersonaDto = daisyControlAddPersonaDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlAddPersonaDto?.Id == null)
            {
                response = "Invalid Dto. Persona Id was invalid. Request payload was incorrect.";
                return false;
            }

            // Get Persona from storage to check if it already exists
            var user = await daisyControlDal.TryGetPersonaAsync(daisyControlAddPersonaDto.Id);

            if (user != null)
            {
                response = $"Persona with id [{daisyControlAddPersonaDto.Id}] already exists.";
                return false;
            }

            // Add the new user
            var userResult = await daisyControlDal.TryAddPersonaAsync(daisyControlAddPersonaDto);

            response = userResult;
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
