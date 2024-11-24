using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlAddUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddUserDto daisyControlAddUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private string response = null;

        public DaisyControlAddUserRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlAddUserDto daisyControlAddUserDto)
        {
            this.daisyControlAddUserDto = daisyControlAddUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlAddUserDto?.Id == null)
            {
                response = "Invalid Dto. Request payload was incorrect.";
                return false;
            }

            // Get User from storage to check if it already exists
            var user = await daisyControlDal.TryGetUserAsync(daisyControlAddUserDto.Id);

            if (user != null)
            {
                response = $"User with id [{daisyControlAddUserDto.Id}] already exists.";
                return false;
            }

            // Add the new user
            await daisyControlDal.TryAddUserAsync(daisyControlAddUserDto);

            response = $"User [{daisyControlAddUserDto.Name}] was correctly added to storage.";
            return true;
        }

        public async Task<string> GetResponseAsync() => response;
    }
}
