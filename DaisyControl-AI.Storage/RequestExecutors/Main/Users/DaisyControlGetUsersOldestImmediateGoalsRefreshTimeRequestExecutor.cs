using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlGetUsersOldestImmediateGoalsRefreshTimeRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUsersWithOldestImmediateGoalsRefreshTimeRequestDto daisyControlGetUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUsersOldestImmediateGoalsRefreshTimeRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetUsersWithOldestImmediateGoalsRefreshTimeRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of users from storage that have unprocessed messages
            var usersToProcess = await daisyControlDal.TryGetUsersWithOldestImmediateGoalsRefreshTimeAsync(daisyControlGetUserDto.MaxNbUsersToFetch);

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
