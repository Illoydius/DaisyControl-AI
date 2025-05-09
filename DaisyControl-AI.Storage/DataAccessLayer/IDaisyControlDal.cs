using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.MessagesBuffer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Response.Messages;
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
        //Task<DaisyControlStorageUserDto[]> TryGetUsersWithMessagesToProcessAsync(int limitRows = 10);

        // Messages Buffer
        Task<DaisyControlGetMessageFromBufferResponseDto> TryGetMessageFromBufferAsync(string messageId);
        Task<DaisyControlAddMessageToBufferDto> TryAddMessageToBufferAsync(DaisyControlAddMessageToBufferDto daisyControlAddMessageToBufferDto);
        Task<DaisyControlMessageToBufferResponseDto[]> TryGetPendingMessagesFromBufferAsync(int limitRows = 5);
    }
}
