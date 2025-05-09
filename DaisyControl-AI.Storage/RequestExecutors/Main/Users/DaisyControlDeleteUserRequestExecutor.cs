using DaisyControl_AI.Common.Exceptions.HTTP;
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;

namespace DaisyControl_AI.Storage.RequestExecutors.Main.Users
{
    public class DaisyControlDeleteUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlDeleteUserRequestDto daisyControlDeleteUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlDeleteUserRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlDeleteUserRequestDto daisyControlDeleteUserDto)
        {
            this.daisyControlDeleteUserDto = daisyControlDeleteUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlDeleteUserDto?.UserId == null)
            {
                response = "Invalid Dto. Request payload was incorrect.";
                return false;
            }

            // Get User from storage to check if it exists
            var user = await daisyControlDal.TryGetUserAsync(daisyControlDeleteUserDto.UserId);

            if(user == null)
            {
                throw new BadRequestWebApiException("e3fa66fe-25c7-4e33-bc1b-e321e986e05b", $"UserId [{daisyControlDeleteUserDto.UserId}] to delete didn't exist in the storage.");
            }

            bool result = await daisyControlDal.TryDeleteUserAsync(daisyControlDeleteUserDto.UserId);
            response = result;
            return result;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
