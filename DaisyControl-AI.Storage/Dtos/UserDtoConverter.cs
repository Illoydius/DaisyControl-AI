using DaisyControl_AI.Storage.Dtos.Response;
using DaisyControl_AI.Storage.Dtos.Storage;

namespace DaisyControl_AI.Storage.Dtos
{
    public static class UserDtoConverter
    {
        public static DaisyControlGetUserResponseDto ConvertStorageUserToGetResponseDto(DaisyControlStorageUserDto user)
        {
            if (user == null)
            {
                return null;
            }

            return new DaisyControlGetUserResponseDto
            {
                CreatedAtUtc = user.CreatedAtUtc,
                Id = user.Id,
                MessagesHistory = user.MessagesHistory,
                LastModifiedAtUtc = user.LastModifiedAtUtc,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Username = user.Username,
                Revision = user.Revision,
                NbUnprocessedMessages = user.NbUnprocessedMessages,
                Status = user.Status,
            };
        }
    }
}
