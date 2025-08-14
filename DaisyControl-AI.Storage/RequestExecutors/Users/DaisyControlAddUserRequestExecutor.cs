using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Users
{
    public class DaisyControlAddUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddUserRequestDto daisyControlAddUserDto = null;
        private IUsersDal daisyControlDal = null;
        private object response = null;

        public DaisyControlAddUserRequestExecutor(
            IUsersDal daisyControlDal,
            DaisyControlAddUserRequestDto daisyControlAddUserDto)
        {
            this.daisyControlAddUserDto = daisyControlAddUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlAddUserDto?.Id == null)
            {
                response = "Invalid Dto. User Id was invalid. Request payload was incorrect.";
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

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
