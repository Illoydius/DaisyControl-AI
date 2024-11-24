using DaisyControl_AI.Common.Diagnostics;

namespace DaisyControl_AI.Common.HttpRequest
{
    public static class CustomHttpClient
    {
        private static readonly HttpClient httpClient = null;

        static CustomHttpClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();

            // Ignore SSL Cert validation
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            httpClient = new HttpClient(clientHandler);
        }

        public static async Task<string> TryGetAsync(string url)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            } catch (HttpRequestException e)
            {
                LoggingManager.LogToFile("4f42bbef-f79a-4075-9d0f-9de8f856f853", $"GET HttpRequest to url [{url}] failed.", e);
            }

            return null;
        }

        public static async Task<string> TryPostAsync(string url, HttpContent payload)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                using HttpResponseMessage response = await httpClient.PostAsync(url, payload);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            } catch (HttpRequestException e)
            {
                LoggingManager.LogToFile("d4b8a37a-c1c1-4a01-a994-db77500854a9", $"POST HttpRequest to url [{url}] failed.", e);
            }

            return null;
        }
    }
}
