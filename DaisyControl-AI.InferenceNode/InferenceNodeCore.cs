using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Common.Utils.MiscUtils;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Core.InferenceServer;
using DaisyControl_AI.Core.InferenceServer.Context;
using DaisyControl_AI.Core.Utils;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.InferenceNode
{
    internal static class InferenceNodeCore
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();

        internal static async Task StartAsync()
        {
            while (true)
            {
                int nbMessagesProcessed = 0;
                try
                {
                    nbMessagesProcessed = await ProcessNextPendingMessage();
                } catch (Exception e)
                {
                    LoggingManager.LogToFile("e4adb2b5-171f-4387-96c8-86634f46709d", $"Failed to process messages batch. Exception: [{ExceptionUtils.BuildExceptionAndInnerExceptionsMessage(e)}].");
                    await Task.Delay(500);
                }

                if (nbMessagesProcessed <= 0)
                {
                    // wait only if there's nothing in buffer
                    await Task.Delay(3000);
                } else
                {
                    await Task.Delay(500);
                    Console.WriteLine($"[{nbMessagesProcessed}] messages processed. Fetching a new batch...");
                }
            }
        }

        private static async Task<int> ProcessNextPendingMessage()
        {
            DaisyControlGetUsersResponseDto result = await usersHttpClient.GetUsersWithUserPendingMessagesAsync(1);

            if (result?.Users == null || !result.Users.Any())
            {
                return 0;
            }

            // Reserve the User for processing
            result.Users[0].NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(30);
            result.Users[0].Status = Storage.Dtos.UserStatus.Working;
            if (!await usersHttpClient.UpdateUserAsync(result.Users[0]))
            {
                LoggingManager.LogToFile("f11fe42e-0a1b-4f14-8441-bd03bdbea791", $"Couldn't reserve User [{result.Users[0].Id}]. Re-queuing.");
                return 0;
            }

            result.Users[0].Revision++;

            LoggingManager.LogToFile("acf897a0-42d0-442a-9667-22750e0cdaad", $"Reserved User [{result.Users[0].Id}] for local processing.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            try
            {
                // Process the next pending message against the running inference server
                await ProcessNextPendingMessage(result.Users[0]);
            } finally
            {
                while (true)
                {
                    var currentUser = await usersHttpClient.GetUserAsync(ulong.Parse(result.Users[0].Id));

                    if (currentUser == null)
                    {
                        LoggingManager.LogToFile("9687817c-2919-4295-bb44-42886f00e9d2", $"User [{result.Users[0].Id}] Status failed to update after processing a message. Retrying...");
                        await Task.Delay(500);
                        continue;
                    }

                    currentUser.Status = UserStatus.Ready;
                    if (await usersHttpClient.UpdateUserAsync(currentUser))
                    {
                        break;
                    }
                }
            }

            return 1;
        }

        private static async Task ProcessNextPendingMessage(DaisyControlUserDto daisyControlUserDto)
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
                return;
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
                return;
            }

            // Clean AI Response
            // TODO If response was null or badly formatted, try again?
            AIresponse.Text = AIMessageUtils.CleanAIResponse(daisyMind, AIresponse.Text);

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
            if ((DateTime.UtcNow - daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc).TotalMinutes >= 5)
            {
                if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 1)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow;
                } else if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count <= 5)
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddSeconds(30);
                } else
                {
                    daisyMind.DaisyMemory.User.Global.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(5);
                }
            }

            // Update DaisyMind (in User)
            bool storageUpdateSuccess = await usersHttpClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global).ConfigureAwait(false);

            if (!storageUpdateSuccess)
            {
                LoggingManager.LogToFile("faeb2139-a43a-4292-978d-48f1272197ab", $"Message from user [{daisyMind.DaisyMemory.User.Global.Id}] was'nt registered properly in storage. The AI will requeue the message from User [{daisyMind.DaisyMemory.User.Global.UserInfo.Username}].");

                // re-queue
                daisyMind.DaisyMemory.User.Global.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;
                await usersHttpClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global);
                return;
            } else
            {
                LoggingManager.LogToFile("18c29818-d646-4e47-9483-35ed05e40593", $"Message from user [{daisyMind.DaisyMemory.User.Global.Id}] was properly processed.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            }
        }
    }
}
