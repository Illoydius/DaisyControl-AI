using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlUpdateUserRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlUpdateUserRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto)
        {
            this.daisyControlUpdateUserDto = daisyControlUpdateUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (daisyControlUpdateUserDto?.Id == null)
            {
                response = "Invalid Dto. User Id was invalid. Request payload was incorrect.";
                return false;
            }

            // Get User from storage to check if it already exists
            var user = await daisyControlDal.TryGetUserAsync(daisyControlUpdateUserDto.Id);

            if (user == null)
            {
                response = $"User with id [{daisyControlUpdateUserDto.Id}] doesn't exists.";
                return false;
            }

            // Update the new user
            var userResult = await daisyControlDal.TryUpdateUserAsync(daisyControlUpdateUserDto);

            response = userResult;
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
