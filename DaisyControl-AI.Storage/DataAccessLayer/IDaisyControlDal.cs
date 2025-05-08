using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public interface IDaisyControlDal
    {
        Task<DaisyControlStorageUserDto> TryGetUserAsync(string userId);
        Task<DaisyControlAddUserRequestDto> TryAddUserAsync(DaisyControlAddUserRequestDto daisyControlAddUserDto);
        Task<DaisyControlUpdateUserRequestDto> TryUpdateUserAsync(DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto);
        Task<bool> TryDeleteUserAsync(string userId);
        Task<DaisyControlStorageUserDto[]> TryGetUsersWithMessagesToProcessAsync(int limitRows = 10);
    }
}
