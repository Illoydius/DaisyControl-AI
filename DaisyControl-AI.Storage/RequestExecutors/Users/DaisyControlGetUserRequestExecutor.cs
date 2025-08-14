using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Users
{
    public class DaisyControlGetUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUserRequestDto daisyControlGetUserDto = null;
        private IUsersDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUserRequestExecutor(
            IUsersDal daisyControlDal,
            DaisyControlGetUserRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlGetUserDto?.UserId == null)
            {
                response = "Invalid Dto. Request payload was incorrect.";
                return false;
            }

            // Get User from storage to check if it already exists
            var user = await daisyControlDal.TryGetUserAsync(daisyControlGetUserDto.UserId);

            response = user;//UserDtoConverter.ConvertStorageUserToGetResponseDto(user);
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
