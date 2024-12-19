using System.Text;
using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Response;

namespace DaisyControl_AI.Common.HttpRequest
{
    public class DaisyControlStorageClient
    {
        public async Task<DaisyControlGetUserResponseDto> GetUserAsync(ulong userId)
        {
            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/storage/users/{userId}";
            var serializedResponse = await CustomHttpClient.TryGetAsync(url).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return null;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlGetUserResponseDto>(serializedResponse);
                return responseDto;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("636232b4-9875-4ced-9e38-1d150d7f7551", $"Failed to deserialize response of type [{typeof(DaisyControlGetUserResponseDto)}] from url [{url}].");
                return null;
            }
        }

        public async Task<DaisyControlAddUserResponseDto> AddUserAsync(ulong userId, string userName)
        {
            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/storage/users";

            DaisyControlAddUserRequestDto requestDto = new()
            {
                Id = userId.ToString(),
                Username = userName,
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            var serializedResponse = await CustomHttpClient.TryPostAsync(url, httpContent).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return null;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlAddUserResponseDto>(serializedResponse);
                return responseDto;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("ebd7781a-779b-45f0-9eff-fb085924ba71", $"Failed to deserialize response of type [{typeof(DaisyControlAddUserResponseDto)}] from url [{url}].");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(DaisyControlUserResponseDto user)
        {
            if (user == null)
            {
                return false;
            }

            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/storage/users/{user.Id}";

            var httpContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var serializedResponse = await CustomHttpClient.TryPutAsync(url, httpContent).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return false;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlUserResponseDto>(serializedResponse);
                return true;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("707b4170-d6c3-4cc0-9f58-41be5f88b3ee", $"Failed to deserialize response of type [{typeof(DaisyControlUserResponseDto)}] from url [{url}].");
                return false;
            }
        }

        public async Task<DaisyControlGetUsersWithUnprocessedMessagesResponseDto> TryGetUsersWithMessagesToProcessAsync()
        {
            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/storage/usersWithUnprocessedMessages?maxNbUsersToFetch=12";
            var serializedResponse = await CustomHttpClient.TryGetAsync(url).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return null;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlGetUsersWithUnprocessedMessagesResponseDto>(serializedResponse);
                return responseDto;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("4a730644-dc30-4072-a026-5aa5d6c658c3", $"Failed to deserialize response of type [{typeof(DaisyControlGetUserResponseDto)}] from url [{url}].");
                return null;
            }
        }
    }
}
