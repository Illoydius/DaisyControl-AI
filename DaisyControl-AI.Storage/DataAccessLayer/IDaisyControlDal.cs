using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public interface IDaisyControlDal
    {
        Task<DaisyControlStorageUserDto> TryGetUserAsync(string userId);
        Task<DaisyControlAddUserRequestDto> TryAddUserAsync(DaisyControlAddUserRequestDto daisyControlAddUserDto);
        Task<DaisyControlUpdateUserRequestDto> TryUpdateUserAsync(DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto);
    }
}
