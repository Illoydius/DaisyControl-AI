using System.Collections.Generic;
using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Common.Utils;
using DaisyControl_AI.Common.Utils.MiscUtils;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Core.InferenceServer;
using DaisyControl_AI.Core.InferenceServer.Context;
using DaisyControl_AI.Core.Utils;
using DaisyControl_AI.InferenceNode.Executors;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;
using Discord;

namespace DaisyControl_AI.InferenceNode
{
    internal static class InferenceNodeCore
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();

        internal static async Task StartAsync()
        {
            while (true)
            {
                int nbTasksProcessed = 0;
                try
                {
                    // Pending messages
                    nbTasksProcessed = await ProcessNextPendingMessage();

                    // Pending inference tasks, such as goals validation
                    nbTasksProcessed += await ProcessNextInferenceTask();

                } catch (Exception e)
                {
                    LoggingManager.LogToFile("e4adb2b5-171f-4387-96c8-86634f46709d", $"Failed to process tasks batch. Exception: [{ExceptionUtils.BuildExceptionAndInnerExceptionsMessage(e)}].");
                    await Task.Delay(500);
                }

                if (nbTasksProcessed <= 0)
                {
                    // wait only if there's nothing in buffer
                    await Task.Delay(3000);
                } else
                {
                    await Task.Delay(500);
                    Console.WriteLine($"[{nbTasksProcessed}] tasks processed. Fetching a new batch...");
                }
            }
        }

        private static async Task<DaisyControlUserDto> ReserveUserForProcessing(DaisyControlUserDto user)
        {
            // Reserve the User for processing
            user.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(30);
            user.Status = Storage.Dtos.UserStatus.Working;
            if (!await usersHttpClient.UpdateUserAsync(user))
            {
                LoggingManager.LogToFile("f11fe42e-0a1b-4f14-8441-bd03bdbea791", $"Couldn't reserve User [{user.Id}]. Re-queuing.");
                return null;
            }

            user.Revision++;

            LoggingManager.LogToFile("acf897a0-42d0-442a-9667-22750e0cdaad", $"Reserved User [{user.Id}] for local processing.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            return user;
        }

        private static async Task<bool> CheckinUserLock(DaisyControlUserDto user, int retries)
        {
            for (int i = 0; i < retries; i++)
            {
                if (user == null)
                {
                    LoggingManager.LogToFile("ca89834d-777a-4722-8dd4-6906729e2ea2", $"User [{user.Id}] Status failed to update after processing a message. Retrying...");
                    await Task.Delay(500);
                    continue;
                }

                user.Status = Storage.Dtos.UserStatus.Ready;
                if (await usersHttpClient.UpdateUserAsync(user))
                {
                    return true;
                } else
                {
                    await Task.Delay(500);
                }
            }

            return false;
        }

        private static async Task<bool> CheckinUserLock(ulong userId, int retries)
        {
            for (int i = 0; i < retries; i++)
            {
                DaisyControlGetUserResponseDto currentUser = await usersHttpClient.GetUserAsync(userId);

                if (currentUser == null)
                {
                    LoggingManager.LogToFile("2bc84435-5f9c-493d-87da-2e6d452714d7", $"User [{userId}] Status failed to update after processing a message. Retrying...");
                    await Task.Delay(500);
                    continue;
                }

                currentUser.Status = Storage.Dtos.UserStatus.Ready;
                if (await usersHttpClient.UpdateUserAsync(currentUser))
                {
                    return true;
                }
            }

            return false;
        }

        private static async Task<int> ProcessNextInferenceTask()
        {
            DaisyControlGetUsersResponseDto result = await usersHttpClient.GetUsersWithUserPendingInferenceTasksAsync(1);

            if (result?.Users == null || !result.Users.Any())
            {
                return 0;
            }

            DaisyControlUserDto user = await ReserveUserForProcessing(result.Users[0]);

            if (user == null)
            {
                return 0;
            }

            try
            {
                // Process the next inference task (such as pending goal validation) against the running inference server
                user = await ProcessNextInferenceTask(user);
            } finally
            {
                if (user != null && !await CheckinUserLock(user, 120))
                {
                    // Failed to save the updated user, just unlock it then
                    await CheckinUserLock(ulong.Parse(user.Id), 1000);
                } else
                {
                    await CheckinUserLock(ulong.Parse(user.Id), 1000);
                }
            }

            return 1;
        }

        private static async Task<DaisyControlUserDto> ProcessNextInferenceTask(DaisyControlUserDto user)
        {
            // The User is locked to us for the next 30 min
            // We want to create the AI "DaisyMind" related to that User. (A User could personalize the bot personality, for instance, so it's unique for each user)
            DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(user).ConfigureAwait(false);

            // We will process all pending inference tasks for this user

            var inferenceTaskToValidate = daisyMind.DaisyMemory.User.Global.InferenceTasks.FirstOrDefault();

            if (inferenceTaskToValidate == null)
            {
                return null;
            }

            daisyMind.DaisyMemory.User.Global.InferenceTasks.Remove(inferenceTaskToValidate);

            // Call the right executor depending on the task KeyType (each executor will build different context depending on the KeyType to query the inference server)
            var inferenceServerQueryerExecutor = InferenceServerQueryerFactory.GenerateExecutor(inferenceTaskToValidate, daisyMind.DaisyMemory.User.Global);

            if (inferenceServerQueryerExecutor == null)
            {
                return daisyMind.DaisyMemory.User.Global;
            }

            string queryJsonResult = await inferenceServerQueryerExecutor.Execute();



            if (!await inferenceServerQueryerExecutor.SaveResult(queryJsonResult))
            {
                LoggingManager.LogToFile("86064c58-e91b-4329-bc5a-748db141ad49", $"Couldn't save AI response after executing inferenceTask against inference server. Task=[{JsonSerializer.Serialize(inferenceTaskToValidate)}], reply=[{queryJsonResult}]. Skipping.");
                return daisyMind.DaisyMemory.User.Global;
            }

            return daisyMind.DaisyMemory.User.Global;
        }

        private static async Task<int> ProcessNextPendingMessage()
        {
            DaisyControlGetUsersResponseDto result = await usersHttpClient.GetUsersWithUserPendingMessagesAsync(1);

            if (result?.Users == null || !result.Users.Any())
            {
                return 0;
            }

            DaisyControlUserDto user = await ReserveUserForProcessing(result.Users[0]);

            if (user == null)
            {
                return 0;
            }

            try
            {
                // Process the next pending message against the running inference server
                user = await ProcessNextPendingMessage(user);
            } finally
            {
                if (user != null && !await CheckinUserLock(user, 120))
                {
                    // Failed to save the updated user, just unlock it then
                    await CheckinUserLock(ulong.Parse(user.Id), 1000);
                } else
                {
                    await CheckinUserLock(ulong.Parse(user.Id), 1000);
                }
            }

            return 1;
        }

        private static async Task<DaisyControlUserDto> ProcessNextPendingMessage(DaisyControlUserDto daisyControlUserDto, bool retry = true)
        {
            // The User is locked to us for the next 30 min
            // We want to create the AI "DaisyMind" related to that User. (A User could personalize the bot personality, for instance, so it's unique for each user)
            DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(daisyControlUserDto).ConfigureAwait(false);

            MessageSourceType replySourceType = MessageSourceType.Channel;
            string replySourceReferential = "";

            DaisyControlMessage sourceMessage = daisyMind.DaisyMemory.User.Global.MessagesHistory.LastOrDefault(w => w.ReferentialType == MessageReferentialType.User && w.MessageStatus == MessageStatus.Pending && w.SourceInfo.MessageSourceType == MessageSourceType.DirectMessage);

            if (sourceMessage == null)
            {
                sourceMessage = daisyMind.DaisyMemory.User.Global.MessagesHistory.LastOrDefault(w => w.ReferentialType == MessageReferentialType.User && w.MessageStatus == MessageStatus.Pending && w.SourceInfo.MessageSourceType == MessageSourceType.Channel);
            }

            if (sourceMessage == null)
            {
                LoggingManager.LogToFile("40027a52-ee98-47cc-a8d0-975171b694bc", $"Couldn't find a way to transmit the AI reply to user [{daisyControlUserDto.Id}].");
                return null;
            }

            // Query the inference server
            string context = ContextBuilder.BuildContext(daisyMind);
            InferenceServerPromptResultResponseDto AIresponse = await InferenceServerQueryer.GenerateStandardAiResponseAsync(context).ConfigureAwait(false);

            if (AIresponse == null)
            {
                // re-queue
                daisyMind.DaisyMemory.User.Global.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;
                await usersHttpClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global);
                LoggingManager.LogToFile("491a17c9-500a-42a4-8fe1-3fe885384e8b", $"AIResponse was NULL in response generation to user [{daisyMind.DaisyMemory.User.Global.Id}]. Aborting AI reply. Re-queuing the pending message.");
                return null;
            }

            // Clean AI Response
            // TODO If response was null or badly formatted, try again?
            AIresponse.Text = AIMessageUtils.CleanAIResponse(daisyMind, AIresponse.Text);
            AIresponse.Text = StringUtils.GetJsonFromString(AIresponse.Text);

            // Check if the AI response is the right format
            try
            {
                JsonSerializer.Deserialize<DaisyMessage>(AIresponse.Text);
            } catch (Exception e)
            {
                if (retry)
                {
                    LoggingManager.LogToFile("4d6d45aa-b958-477a-977d-9110a4dd3a4d", "AI reply is in an incorrect format. Retrying...");
                    return await ProcessNextPendingMessage(daisyControlUserDto, false);
                }

                LoggingManager.LogToFile("5c97dd19-3485-499a-9b6a-10dcffd3d0b7", $"AI reply is in an incorrect format! The response will be kept as-is: [{AIresponse.Text}].");

                AIresponse.Text = JsonSerializer.Serialize(new DaisyMessage
                {
                    Message = AIresponse.Text,
                });
            }

            // Clean up pending messages to set as processed
            foreach (var message in daisyMind.DaisyMemory.User.Global.MessagesHistory.Where(w => w.MessageStatus == MessageStatus.Pending))
            {
                message.MessageStatus = MessageStatus.Processed;
            }

            // Add the AI response
            daisyMind.DaisyMemory.User.Global.MessagesHistory.Add(new DaisyControlMessage
            {
                CreatedAtUtc = DateTime.UtcNow,
                ReferentialType = MessageReferentialType.Assistant,
                MessageContent = AIresponse.Text,
                MessageStatus = MessageStatus.Pending,
                SourceInfo = new()
                {
                    MessageSource = sourceMessage.SourceInfo.MessageSource,
                    MessageSourceType = sourceMessage.SourceInfo.MessageSourceType,
                    MessageSourceReferential = sourceMessage.SourceInfo.MessageSourceReferential,
                },
            });

            daisyMind.DaisyMemory.User.Global.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;

            // Also update the immediate goal for the AI to soon-ish as we just received a message
            if ((daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc - DateTime.UtcNow).TotalMinutes >= 5)
            {
                if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 1)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow;
                } else if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 5)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddSeconds(30);
                } else if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 20)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(1);
                } else if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 50)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(2);
                } else
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(5);
                }
            }

            return daisyMind.DaisyMemory.User.Global;
            // Update DaisyMind (in User)
            //daisyMind.DaisyMemory.User.Global.Status = UserStatus.Ready;
            //bool storageUpdateSuccess = await usersHttpClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global).ConfigureAwait(false);

            //if (!storageUpdateSuccess)
            //{
            //    LoggingManager.LogToFile("faeb2139-a43a-4292-978d-48f1272197ab", $"Message from user [{daisyMind.DaisyMemory.User.Global.Id}] was'nt registered properly in storage. The AI will requeue the message from User [{daisyMind.DaisyMemory.User.Global.UserInfo.Username}].");

            //    // re-queue
            //    daisyMind.DaisyMemory.User.Global.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;
            //    await usersHttpClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global);
            //    return;
            //} else
            //{
            //    LoggingManager.LogToFile("18c29818-d646-4e47-9483-35ed05e40593", $"Message from user [{daisyMind.DaisyMemory.User.Global.Id}] was properly processed.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            //}
        }
    }
}
