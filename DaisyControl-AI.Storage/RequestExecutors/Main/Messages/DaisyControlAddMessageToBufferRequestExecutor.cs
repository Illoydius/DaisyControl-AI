using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Errors;

namespace DaisyControl_AI.Storage.RequestExecutors.Main.Messages
{
    /// <summary>
    /// Code to add a message to the buffer of messages available to process by workers.
    /// </summary>
    public class DaisyControlAddMessageToBufferRequestExecutor : IMainRequestExecutor
    {
        private DaisyControlAddMessageToBufferDto daisyControlAddMessageToBufferDto = null;
        private IDaisyControlDal daisyControlDal = null;
        private object response = null;

        public DaisyControlAddMessageToBufferRequestExecutor(
            IDaisyControlDal daisyControlDal,
            DaisyControlAddMessageToBufferDto daisyControlAddMessageToBufferDto)
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

            // When we're trying to add a message to a user that doesn't exist, create the new user
            if (user == null)
            {
                response = new ResponseError()
                {
                    ErrorCode = StorageResponseErrorCodes.DaisyControlAddMessageToBufferRequestExecutor_UserNotFound,
                    ErrorMessage = "User didn't exists. The messages can't be added to an invalid user.",
                }.AsJson();
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
