using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Response;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlGetUsersWithUnprocessedMessagesRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetUsersWithUnprocessedMessagesRequestDto daisyControlGetUserDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetUsersWithUnprocessedMessagesRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetUsersWithUnprocessedMessagesRequestDto daisyControlGetUserDto)
        {
            this.daisyControlGetUserDto = daisyControlGetUserDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of users from storage that have unprocessed messages
            var usersToProcess = await daisyControlDal.TryGetUsersWithMessagesToProcessAsync(daisyControlGetUserDto.MaxNbUsersToFetch);

            if (usersToProcess == null)
            {
                return true;
            }

            response = new DaisyControlGetUsersWithUnprocessedMessagesResponseDto
            {
                Users = usersToProcess.Select(UserDtoConverter.ConvertStorageUserToGetResponseDto).ToArray(),
            };

            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
