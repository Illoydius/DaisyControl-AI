using DaisyControl_AI.Core.InferenceServer.Context;
using DaisyControl_AI.Core.InferenceServer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos.User;
using DaisyControl_AI.Common.Diagnostics;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using System.Text.RegularExpressions;
using DaisyControl_AI.Common.Utils;

namespace DaisyControl_AI.InferenceNode.Executors
{
    internal class DaisyControlGoalValidationRequestExecutor : IInferenceServerQueryerExecutor
    {
        private InferenceTask inferenceTask = null;
        private DaisyControlUserDto daisyControlUserDto = null;

        public DaisyControlGoalValidationRequestExecutor(InferenceTask inferenceTask, DaisyControlUserDto daisyControlUserDto)
        {
            this.inferenceTask = inferenceTask;
            this.daisyControlUserDto = daisyControlUserDto;
        }

        public async Task<string> Execute()
        {
            DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(daisyControlUserDto).ConfigureAwait(false);

            string context = GoalValidationContextBuilder.BuildContext(daisyMind, daisyControlUserDto, inferenceTask.KeyValue, inferenceTask.Prompt);
            InferenceServerPromptResultResponseDto AIresponse = await InferenceServerQueryer.GenerateStandardAiResponseAsync(context).ConfigureAwait(false);

            return AIresponse?.Text;
        }

        public async Task<bool> SaveResult(string queryJsonResult)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(queryJsonResult))
                {
                    return false;
                }

                var jsonAdapted = queryJsonResult.ToLowerInvariant().Trim();
                jsonAdapted = queryJsonResult.Replace($"\"{inferenceTask.KeyValue}\"", "\"value\"");

                string pattern = @"\{[\s\S]*?\}";

                Match match = Regex.Match(jsonAdapted, pattern);
                if (!match.Success || match.Groups.Count <= 0)
                {
                    // No json found
                    return false;
                }

                var result = JsonSerializer.Deserialize<ExecutorQueryResult>(match.Groups[0].Value);

                if (result == null || string.IsNullOrWhiteSpace(result.Value))
                {
                    return false;
                }

                return await SaveMemoryUpdate(inferenceTask.KeyType, inferenceTask.KeyValue, result.Value);

            } catch (Exception e)
            {
                LoggingManager.LogToFile("01b409ef-d5e8-4b4d-adb1-9d347e5460d7", $"Couldn't deserialize AI response after executing inferenceTask against inference server. Task=[{JsonSerializer.Serialize(inferenceTask)}], reply=[{queryJsonResult}]. Skipping.");
                return false;
            }
        }

        private async Task<bool> SaveMemoryUpdate(InferenceTaskKeyType keyType, string keyValue, string value)
        {
            switch (keyType)
            {
                case InferenceTaskKeyType.GoalValidation:
                    return await SaveGoalMemoryUpdate(keyValue, value);
                default:
                    LoggingManager.LogToFile("2a648fc6-1aad-479e-bed0-b08a316c6f30", $"Unhandled {nameof(InferenceTaskKeyType)} [{keyType}].");
                    return false;
            }
        }

        private async Task<bool> SaveGoalMemoryUpdate(string keyValue, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.ToLowerInvariant() == "unknown")
            {
                return false;
            }

            switch (keyValue)
            {
                case "firstname":
                    daisyControlUserDto.UserInfo.FirstName = value.CapitalizeFirstLetter();
                    break;
                case "lastname":
                    daisyControlUserDto.UserInfo.LastName = value.CapitalizeFirstLetter();
                    break;
                case "age":
                    if (!int.TryParse(value, out int intValue))
                    {
                        LoggingManager.LogToFile("e84b1f31-a2bf-4c1a-a97d-1b1b12a7f43b", $"AI found an invalid Age for user [{value}]. Skipping.");
                        return false;
                    }

                    daisyControlUserDto.UserInfo.Age = intValue;
                    break;
                case "gender":
                    if (!Enum.TryParse(value, true, out Gender genderValue))
                    {
                        LoggingManager.LogToFile("76829821-a90d-449a-9e0a-966122815a5e", $"AI found an invalid Gender for user [{value}]. Skipping.");
                        return false;
                    }

                    daisyControlUserDto.UserInfo.Gender = genderValue;
                    break;
                case "genitals":
                    if (!Enum.TryParse(value, true, out Genitals genitalsValue))
                    {
                        LoggingManager.LogToFile("04a7cddc-0305-4480-a821-802d93102154", $"AI found invalid Genitals for user [{value}]. Skipping.");
                        return false;
                    }

                    daisyControlUserDto.UserInfo.Genitals = genitalsValue;
                    break;
                case "worktitle":
                    daisyControlUserDto.UserInfo.WorkOccupation.WorkTitle = value.CapitalizeFirstLetter();
                    break;
                default:
                    LoggingManager.LogToFile("30fd0d34-5682-4582-86c8-3b7abc807da3", $"Unhandled goal keyValue [{keyValue}].");
                    return false;
            }

            return true;
        }
    }
}
