using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;

namespace DaisyControl_AI.Storage.RequestExecutors.Main.Users
{
    public class DaisyControlGetUsersWithFollowUpsRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUsersWithFollowUpsRequestDto daisyControlGetUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUsersWithFollowUpsRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetUsersWithFollowUpsRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of users from storage that have pending inference tasks
            var usersToProcess = await daisyControlDal.TryGetUsersWithFollowUpsRequestsAsync(daisyControlGetUserDto.MaxNbUsersToFetch);

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
