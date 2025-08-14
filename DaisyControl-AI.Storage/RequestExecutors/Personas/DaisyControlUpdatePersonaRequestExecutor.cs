using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Personas
{
    public class DaisyControlUpdatePersonaRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlUpdatePersonaRequestDto daisyControlUpdatePersonaDto = null;
        private IPersonasDal daisyControlDal = null;
        private object response = null;

        public DaisyControlUpdatePersonaRequestExecutor(
            IPersonasDal daisyControlDal,
            DaisyControlUpdatePersonaRequestDto daisyControlUpdatePersonaDto)
        {
            this.daisyControlUpdatePersonaDto = daisyControlUpdatePersonaDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlUpdatePersonaDto?.Id == null)
            {
                response = "Invalid Dto. Persona Id was invalid. Request payload was incorrect.";
                return false;
            }

            // Get Persona from storage to check if it already exists
            var user = await daisyControlDal.TryGetPersonaAsync(daisyControlUpdatePersonaDto.Id);

            if (user == null)
            {
                response = $"Persona with id [{daisyControlUpdatePersonaDto.Id}] doesn't exists.";
                return false;
            }

            // Update the new user
            var userResult = await daisyControlDal.TryUpdatePersonaAsync(daisyControlUpdatePersonaDto);

            response = userResult;
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
