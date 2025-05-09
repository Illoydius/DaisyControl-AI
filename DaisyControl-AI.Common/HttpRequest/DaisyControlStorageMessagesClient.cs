using System.Text;
using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Errors;
using DaisyControl_AI.Storage.Dtos.Response.Users;

namespace DaisyControl_AI.Common.HttpRequest
{
    public class DaisyControlStorageMessagesClient
    {
        string messagesUrl = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/messagesbuffer";

        //public async Task<DaisyControlGetUserResponseDto> GetUserAsync(ulong userId)
        //{
        //    string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/users/{userId}";
        //    var serializedResponse = await CustomHttpClient.TryGetAsync(url).ConfigureAwait(false);

        //    if (string.IsNullOrWhiteSpace(serializedResponse))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        var responseDto = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(serializedResponse);
        //        return responseDto;
        //    } catch (Exception e)
        //    {
        //        LoggingManager.LogToFile("636232b4-9875-4ced-9e38-1d150d7f7551", $"Failed to deserialize response of type [{typeof(DaisyControlGetUserResponseDto)}] from url [{url}].");
        //        return null;
        //    }
        //}

        public async Task<DaisyControlAddMessageToBufferDto> AddMessageToBufferAsync(ulong userId, string userName, DaisyControlAddMessageToBufferDto daisyControlAddMessageToBufferRequestDto, bool createUserIfNotExists = true)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(daisyControlAddMessageToBufferRequestDto), Encoding.UTF8, "application/json");
            var serializedResponse = await CustomHttpClient.TryPostAsync(messagesUrl, httpContent).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return null;
            }

            try
            {
                var responseError = serializedResponse.AsResponseError();
                if (responseError != null && responseError.ValidErrorCode())
                {
                    if (responseError.ErrorCode == StorageResponseErrorCodes.DaisyControlAddMessageToBufferRequestExecutor_UserNotFound && createUserIfNotExists)
                    {
                        // Add the new user
                        var httpRequestMessagesClient = new DaisyControlStorageUsersClient();
                        await httpRequestMessagesClient.AddUserAsync(userId, userName);

                        // Redo the add message
                        return await AddMessageToBufferAsync(userId, userName, daisyControlAddMessageToBufferRequestDto, createUserIfNotExists = false);
                    }
                }
            } catch (Exception)
            {
                // silent
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlAddMessageToBufferDto>(serializedResponse);

                // TODO: validate responseDto

                return responseDto;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("5d38c1c0-7832-498d-a587-6722a4e6d2a9", $"Failed to deserialize response of type [{typeof(DaisyControlAddUserResponseDto)}] from adding new message to storage. Serialized response: [{serializedResponse}].");
                return null;
            }
        }

        //public async Task<bool> UpdateUserAsync(DaisyControlUserResponseDto user)
        //{
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/users/{user.Id}";

        //    var httpContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
        //    var serializedResponse = await CustomHttpClient.TryPutAsync(url, httpContent).ConfigureAwait(false);

        //    if (string.IsNullOrWhiteSpace(serializedResponse))
        //    {
        //        return false;
        //    }

        //    try
        //    {
        //        var responseDto = JsonSerializer.Deserialize<DaisyControlUserResponseDto>(serializedResponse);
        //        return true;
        //    } catch (Exception e)
        //    {
        //        LoggingManager.LogToFile("707b4170-d6c3-4cc0-9f58-41be5f88b3ee", $"Failed to deserialize response of type [{typeof(DaisyControlUserResponseDto)}] from url [{url}].");
        //        return false;
        //    }
        //}
    }
}
