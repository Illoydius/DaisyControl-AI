using System.Text;
using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Dtos.Response.Users;

namespace DaisyControl_AI.Common.HttpRequest
{
    public class DaisyControlStorageUsersClient
    {
        string usersUrl = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/users";

        public async Task<DaisyControlGetUserResponseDto> GetUserAsync(ulong userId)
        {
            string url = $"{usersUrl}/{userId}";
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
            DaisyControlAddUserRequestDto requestDto = new()
            {
                Id = userId.ToString(),
                UserInfo = new()
                {
                    Username = userName,
                }
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            var serializedResponse = await CustomHttpClient.TryPostAsync(usersUrl, httpContent).ConfigureAwait(false);

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
                LoggingManager.LogToFile("ebd7781a-779b-45f0-9eff-fb085924ba71", $"Failed to deserialize response of type [{typeof(DaisyControlAddUserResponseDto)}] from url [{usersUrl}].");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(DaisyControlUserDto user)
        {
            if (user == null)
            {
                return false;
            }

            string url = $"{usersUrl}/{user.Id}";

            var httpContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var serializedResponse = await CustomHttpClient.TryPutAsync(url, httpContent).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return false;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlUserDto>(serializedResponse);
                return true;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("707b4170-d6c3-4cc0-9f58-41be5f88b3ee", $"Failed to deserialize response of type [{typeof(DaisyControlUserDto)}] from url [{url}].");
                return false;
            }
        }

        public async Task<DaisyControlGetUsersWithUnprocessedMessagesResponseDto> GetUsersWithUserPendingMessagesAsync(int limitNbUsersToFetch = 3)
        {
            string url = $"{usersUrl}/unprocessedUsersMessages?maxNbUsersToFetch={limitNbUsersToFetch}";
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
                LoggingManager.LogToFile("636232b4-9875-4ced-9e38-1d150d7f7551", $"Failed to deserialize response of type [{typeof(DaisyControlGetUsersWithUnprocessedMessagesResponseDto)}] from url [{url}].");
                return null;
            }
        }

        public async Task<DaisyControlGetUsersWithUnprocessedMessagesResponseDto> GetUsersWithAIPendingMessagesAsync(int limitNbUsersToFetch = 3)
        {
            string url = $"{usersUrl}/unprocessedAIMessages?maxNbUsersToFetch={limitNbUsersToFetch}";
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
                LoggingManager.LogToFile("636232b4-9875-4ced-9e38-1d150d7f7551", $"Failed to deserialize response of type [{typeof(DaisyControlGetUsersWithUnprocessedMessagesResponseDto)}] from url [{url}].");
                return null;
            }
        }

        public async Task<DaisyControlGetUsersOfStatusWorkingResponseDto> GetWorkingStatusUsersAsync(int limitNbUsersToFetch = 3)
        {
            string url = $"{usersUrl}/working?maxNbUsersToFetch={limitNbUsersToFetch}";
            var serializedResponse = await CustomHttpClient.TryGetAsync(url).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serializedResponse))
            {
                return null;
            }

            try
            {
                var responseDto = JsonSerializer.Deserialize<DaisyControlGetUsersOfStatusWorkingResponseDto>(serializedResponse);
                return responseDto;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("636232b4-9875-4ced-9e38-1d150d7f7551", $"Failed to deserialize response of type [{typeof(DaisyControlGetUsersOfStatusWorkingResponseDto)}] from url [{url}].");
                return null;
            }
        }
    }
}
