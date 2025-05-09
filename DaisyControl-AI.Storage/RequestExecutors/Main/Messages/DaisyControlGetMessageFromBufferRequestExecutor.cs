using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Requests.MessagesBuffer;

namespace DaisyControl_AI.Storage.RequestExecutors.Main.Messages
{
    /// <summary>
    /// Code to get a specific message from the buffer of messages available to process by workers.
    /// </summary>
    public class DaisyControlGetMessageFromBufferRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlGetMessageFromBufferRequestDto daisyControlGetMessageFromBufferDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlGetMessageFromBufferRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlGetMessageFromBufferRequestDto daisyControlGetMessageFromBufferDto)
        {
            this.daisyControlGetMessageFromBufferDto = daisyControlGetMessageFromBufferDto;
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (string.IsNullOrWhiteSpace(daisyControlGetMessageFromBufferDto?.MessageId))
            {
                response = "Invalid Dto. Message Id was invalid. Request payload was incorrect.";
                return false;
            }

            // Get message from storage
            var messageResult = await daisyControlDal.TryGetMessageFromBufferAsync(daisyControlGetMessageFromBufferDto.MessageId);
            response = messageResult;

            return messageResult != null;
        }

        public async Task<object> GetResponseAsync() => await Task.FromResult(response);
    }
}
