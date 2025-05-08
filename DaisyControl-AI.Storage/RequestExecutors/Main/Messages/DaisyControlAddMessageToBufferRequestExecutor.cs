using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.Messages;

namespace DaisyControl_AI.Storage.RequestExecutors.Main.Messages
{
    /// <summary>
    /// Code to add a message to the buffer of messages available to process by workers.
    /// </summary>
    public class DaisyControlAddMessageToBufferRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddMessageToBufferRequestDto daisyControlAddMessageToBufferDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlAddMessageToBufferRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlAddMessageToBufferRequestDto daisyControlAddMessageToBufferDto)
        {
            this.daisyControlAddMessageToBufferDto = daisyControlAddMessageToBufferDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (string.IsNullOrWhiteSpace(daisyControlAddMessageToBufferDto?.UserId))
            {
                response = "Invalid Dto. User Id was invalid. Request payload was incorrect.";
                return false;
            }

            // Get User from storage to validate that we're adding a message referencing a valid user
            var user = await daisyControlDal.TryGetUserAsync(daisyControlAddMessageToBufferDto.UserId);

            if (user == null)
            {
                response = $"User with id [{daisyControlAddMessageToBufferDto.UserId}] doesn't exists. Can't add message to an invalid user.";
                return false;
            }

            // Add the new message
            var messageResult = await daisyControlDal.TryAddMessageToBufferAsync(daisyControlAddMessageToBufferDto);

            response = messageResult;
            return true;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
