using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Users
{
    public static class UsersRequestExecutorFactory
    {
        public static IMainRequestExecutor GenerateExecutor(IUsersDal daisyControlDal, IStorageDto postDto)
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
                case DaisyControlGetUsersWithUnprocessedUserMessagesRequestDto daisyControlGetUsersWithUnprocessedUserMessagesRequestDto:
                    return new DaisyControlGetUsersWithUnprocessedUserMessagesRequestExecutor(daisyControlDal, daisyControlGetUsersWithUnprocessedUserMessagesRequestDto);
                case DaisyControlGetUsersWithUnprocessedAIMessagesRequestDto daisyControlGetUsersWithUnprocessedAIMessagesRequestDto:
                    return new DaisyControlGetUsersWithUnprocessedAIMessagesRequestExecutor(daisyControlDal, daisyControlGetUsersWithUnprocessedAIMessagesRequestDto);
                case DaisyControlGetUsersWithWorkingStatusRequestDto daisyControlGetUsersWithWorkingStatusRequestDto:
                    return new DaisyControlGetUsersWithWorkingStatusRequestExecutor(daisyControlDal, daisyControlGetUsersWithWorkingStatusRequestDto);
                case DaisyControlGetUsersWithOldestThoughtAboutRefreshTimeRequestDto daisyControlGetUsersWithOldestThoughtAboutRefreshTimeRequestDto:
                    return new DaisyControlGetUsersOldestThoughtAboutRefreshTimeRequestExecutor(daisyControlDal, daisyControlGetUsersWithOldestThoughtAboutRefreshTimeRequestDto);
                case DaisyControlGetUsersWithInferenceTasksRequestDto daisyControlGetUsersWithInferenceTasksRequestDto:
                    return new DaisyControlGetUsersWithInferenceTasksRequestExecutor(daisyControlDal, daisyControlGetUsersWithInferenceTasksRequestDto);
                case DaisyControlGetUsersWithFollowUpsRequestDto daisyControlGetUsersWithFollowUpsRequestDto:
                    return new DaisyControlGetUsersWithFollowUpsRequestExecutor(daisyControlDal, daisyControlGetUsersWithFollowUpsRequestDto);

                default:
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
