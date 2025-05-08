using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Response;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public class DaisyControlGetPendingMessagesFromBufferRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetPendingMessagesRequestDto daisyControlGetPendingMessagesDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetPendingMessagesFromBufferRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetPendingMessagesRequestDto daisyControlGetPendingMessagesDto)
        {
            this.daisyControlGetPendingMessagesDto = daisyControlGetPendingMessagesDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Get chunk of messages from storage that are unprocessed
            var messagesToProcess = await daisyControlDal.TryGetPendingMessagesFromBufferAsync(daisyControlGetPendingMessagesDto.MaxNbPendingMessagesToFetch);

            if (messagesToProcess == null)
            {
                response = new DaisyControlGetPendingMessagesFromBufferResponseDto
                {
                    Messages = []// empty collection, we don't want to return NotFound if no pending messages were found
                };

                return true;
            }

            response = new DaisyControlGetPendingMessagesFromBufferResponseDto
            {
                Messages = messagesToProcess//.Select(UserDtoConverter.ConvertStorageUserToGetResponseDto).ToArray(),
            };

            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
