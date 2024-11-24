using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlAddUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddUserRequestDto daisyControlAddUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlAddUserRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlAddUserRequestDto daisyControlAddUserDto)
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
            var userResult = await daisyControlDal.TryAddUserAsync(daisyControlAddUserDto);

            response = userResult;
            return true;
        }

        public async Task<object> GetResponseAsync() => response;
    }
}
