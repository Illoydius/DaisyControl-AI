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
        Task<DaisyControlGetUsersWithUnprocessedMessagesResponseDto> TryGetUsersWithUserMessagesToProcessAsync(int limitRows);
        Task<DaisyControlGetUsersWithUnprocessedMessagesResponseDto> TryGetUsersWithAIMessagesToProcessAsync(int limitRows);
    }
}
