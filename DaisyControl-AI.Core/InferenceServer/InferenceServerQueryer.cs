using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;

namespace DaisyControl_AI.Core.InferenceServer
{
    /// <summary>
    /// Serves to query an inferenceServer, local or remote.
    /// </summary>
    public static class InferenceServerQueryer
    {
        private static int maxContext = 0;
        private static int maxQueryLength = 0;

        static InferenceServerQueryer()
        {
            var config = CommonConfigurationManager.ReloadConfig();

            maxContext = config.InferenceServerConfiguration.MaxContextLength;
            maxQueryLength = config.InferenceServerConfiguration.MaxTokensToGenerateInSingleQueryLength;
        }

        private static InferenceServerPromptResponseDto ParseMessageFromInferenceServerResponse(string responseDto)
        {
            if (responseDto == null)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<InferenceServerPromptResponseDto>(responseDto);
            } catch (Exception e)
            {
                LoggingManager.LogToFile("c6db478e-e854-4e86-a1fc-f0a84ef4c236", $"Couldn't deserialize prompt model from inference server. obj = [{responseDto}].");
                return null;
            }
        }

        public static async Task<InferenceServerPromptResultResponseDto> GenerateStandardAiResponseAsync(string context)
        {
            var config = CommonConfigurationManager.ReloadConfig();

            PromptGenerationRequestDto requestDto = new()
            {
                MaxContextLength = maxContext,
                MaxQueryLength = maxQueryLength,
                PromptContextContent = context,
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            string queryResult = await CustomHttpClient.TryPostAsync(config.InferenceServerConfiguration.UrlGeneratePrompt, httpContent).ConfigureAwait(false);
            InferenceServerPromptResultResponseDto AIResponse = ParseMessageFromInferenceServerResponse(queryResult)?.Results?.FirstOrDefault();
            return AIResponse;
        }
    }
}
