using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Users
{
    public class DaisyControlGetUsersWithUnprocessedAIMessagesRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUsersWithUnprocessedAIMessagesRequestDto daisyControlGetUserDto = null;
        private IUsersDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUsersWithUnprocessedAIMessagesRequestExecutor(
            IUsersDal daisyControlDal,
            DaisyControlGetUsersWithUnprocessedAIMessagesRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of users from storage that have unprocessed messages
            var usersToProcess = await daisyControlDal.TryGetUsersWithAIMessagesToProcessAsync(daisyControlGetUserDto.MaxNbUsersToFetch);

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
