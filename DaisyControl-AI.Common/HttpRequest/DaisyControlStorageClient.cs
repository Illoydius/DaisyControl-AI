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
            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/main/users/{userId}";
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
            string url = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/main/users";

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
    }
}
