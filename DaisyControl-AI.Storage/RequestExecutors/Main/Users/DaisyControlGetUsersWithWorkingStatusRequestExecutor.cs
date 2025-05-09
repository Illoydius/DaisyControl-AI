using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlGetUsersWithWorkingStatusRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUsersWithWorkingStatusRequestDto daisyControlGetUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUsersWithWorkingStatusRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetUsersWithWorkingStatusRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of users from storage that have unprocessed messages
            var usersToProcess = await daisyControlDal.TryGetUsersWithWorkingStatusAsync(daisyControlGetUserDto.MaxNbUsersToFetch);

            if (usersToProcess == null)
            {
                return true;
            }

            response = usersToProcess;
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
