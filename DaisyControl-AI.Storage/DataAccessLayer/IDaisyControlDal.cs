using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Response.Users;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public interface IDaisyControlDal
    {
        // Users
        Task<DaisyControlGetUserResponseDto> TryGetUserAsync(string userId);
        Task<DaisyControlAddUserRequestDto> TryAddUserAsync(DaisyControlAddUserRequestDto daisyControlAddUserDto);
        Task<DaisyControlUpdateUserRequestDto> TryUpdateUserAsync(DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto);
        Task<bool> TryDeleteUserAsync(string userId);
        Task<DaisyControlGetUsersResponseDto> TryGetUsersWithUserMessagesToProcessAsync(int limitRows);
        Task<DaisyControlGetUsersResponseDto> TryGetUsersWithAIMessagesToProcessAsync(int limitRows);
        Task<DaisyControlGetUsersResponseDto> TryGetUsersWithWorkingStatusAsync(int limitRows);
        Task<DaisyControlGetUsersResponseDto> TryGetUsersWithOldestImmediateGoalsRefreshTimeAsync(int limitRows);
        Task<DaisyControlGetUsersResponseDto> TryGetUsersWithPendingInferenceTasksAsync(int limitRows);
    }
}
