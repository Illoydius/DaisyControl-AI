using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.MessagesBuffer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main.Messages;
using DaisyControl_AI.Storage.RequestExecutors.Main.Users;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public static class MainRequestExecutorFactory
    {
        public static IMainRequestExecutor GenerateExecutor(IDaisyControlDal daisyControlDal, IStorageDto postDto)
        {
            switch (postDto)
            {
                // Users
                case DaisyControlAddUserRequestDto daisyControlAddUserDto:
                    return new DaisyControlAddUserRequestExecutor(daisyControlDal, daisyControlAddUserDto);
                case DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto:
                    return new DaisyControlUpdateUserRequestExecutor(daisyControlDal, daisyControlUpdateUserDto);
                case DaisyControlGetUserRequestDto daisyControlGetUserDto:
                    return new DaisyControlGetUserRequestExecutor(daisyControlDal, daisyControlGetUserDto);
                case DaisyControlDeleteUserRequestDto daisyControlDeleteUserDto:
                    return new DaisyControlDeleteUserRequestExecutor(daisyControlDal, daisyControlDeleteUserDto);
                //case DaisyControlGetUsersWithUnprocessedMessagesRequestDto daisyControlGetUsersWithUnprocessedMessagesRequestDto:
                //    return new DaisyControlGetUsersWithUnprocessedMessagesRequestExecutor(daisyControlDal, daisyControlGetUsersWithUnprocessedMessagesRequestDto);
                // MessagesBuffer
                case DaisyControlGetMessageFromBufferRequestDto daisyControlGetMessageFromBufferRequestDto:
                    return new DaisyControlGetMessageFromBufferRequestExecutor(daisyControlDal, daisyControlGetMessageFromBufferRequestDto);
                case DaisyControlAddMessageToBufferDto daisyControlAddMessageToBufferDto:
                    return new DaisyControlAddMessageToBufferRequestExecutor(daisyControlDal, daisyControlAddMessageToBufferDto);
                case DaisyControlGetPendingMessagesRequestDto daisyControlGetPendingMessagesRequestDto:
                    return new DaisyControlGetPendingMessagesFromBufferRequestExecutor(daisyControlDal, daisyControlGetPendingMessagesRequestDto);
                default:
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
