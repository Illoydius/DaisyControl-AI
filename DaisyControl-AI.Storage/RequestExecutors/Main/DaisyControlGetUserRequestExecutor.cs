using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlGetUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUserRequestDto daisyControlGetUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUserRequestExecutor(
            IDaisyControlDal daisyControlDal,
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

            response = UserDtoConverter.ConvertStorageUserToGetResponseDto(user);
            return true;
        }

        public async Task<object> GetResponseAsync() => response;
    }
}
