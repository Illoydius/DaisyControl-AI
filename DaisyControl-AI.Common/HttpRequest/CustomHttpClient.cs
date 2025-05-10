using DaisyControl_AI.Common.Diagnostics;

namespace DaisyControl_AI.Common.HttpRequest
{
    /// <summary>
    /// Wrapper around HTTP Client for simplified use cases and integration within Daisy Control.
    /// </summary>
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
            } catch (HttpRequestException e) when (e.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                LoggingManager.LogToFile("b5013697-8bd6-400f-9e4f-ed40c08dab6c", $"GET HttpRequest to url [{url}] failed.", e);
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

        public static async Task<string> TryPutAsync(string url, StringContent payload)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                using HttpResponseMessage response = await httpClient.PutAsync(url, payload);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            } catch (HttpRequestException e)
            {
                LoggingManager.LogToFile("1c2f9f7f-cc27-4c2e-9ed5-d8b229372107", $"PUT HttpRequest to url [{url}] failed.", e);
            }

            return null;
        }

        public static async Task<string> TryDeleteAsync(string url, StringContent httpContent)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                using HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            } catch (HttpRequestException e)
            {
                LoggingManager.LogToFile("82b4c654-1d31-4b19-a710-407d843f33ba", $"DELETE HttpRequest to url [{url}] failed.", e);
            }

            return null;
        }
    }
}
