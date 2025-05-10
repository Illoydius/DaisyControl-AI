using System.Diagnostics;
using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;
using Microsoft.Extensions.Hosting;

namespace DaisyControl_AI.Core.Comms.Discord
{
    /// <summary>
    /// Background worker on Discord interactions.
    /// Handles AI communications From/To discord.
    /// TODO: handle files(pictures, txt, etc).
    /// </summary>
    public class DiscordWorker : BackgroundService
    {
        private static Stopwatch typingStopWatchTiming = new();
        private static Stopwatch sendMessagegStopWatchTiming = new();
        private static DaisyControlStorageUsersClient usersHttpClient = new();
        private readonly IDaisyControlDiscordClient discordClient;

        public DiscordWorker(
            IDaisyControlDiscordClient discordClient)// The discordClient Ctor will auto subscribe on new messages
        {
            this.discordClient = discordClient;
        }

        /// <summary>
        /// Worker main operative loop.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = CommonConfigurationManager.ReloadConfig();
            if (config?.DiscordBotConfiguration?.Enabled == null || !config.DiscordBotConfiguration.Enabled)
            {
                return;
            }

            typingStopWatchTiming.Start();
            sendMessagegStopWatchTiming.Start();
            while (true)
            {
                try
                {
                    if (discordClient != null && discordClient.IsConnected)
                    {
                        await BackgroundWork();
                    }
                } catch (Exception exception)
                {
                    LoggingManager.LogToFile("8708b99a-0030-419f-9f82-d65c185be004", $"Unhandled exception in discord bot worker main loop.", exception);
                }

                await Task.Delay(500);
            }
        }

        /// <summary>
        /// Stop the connection with Discord Gateway.
        /// </summary>
        public async Task OnShutdownAsync()
        {
            await discordClient?.StopAsync();
        }

        /// <summary>
        /// Background thread running concurrently to handle backend processes.
        /// </summary>
        private async Task BackgroundWork()
        {
            // Check if there's messages to send
            if (sendMessagegStopWatchTiming.ElapsedMilliseconds >= 3000)
            {
                await HandleMessagesToSend();
                sendMessagegStopWatchTiming.Restart();
            }

            // Check if there are inference being worked on that is a reply to a Discord user
            if (typingStopWatchTiming.ElapsedMilliseconds >= 5000)
            {
                await HandleMessagesToShowTyping();
                typingStopWatchTiming.Restart();
            }
        }

        private async Task HandleMessagesToSend()
        {
            DaisyControlGetUsersResponseDto result = await usersHttpClient.GetUsersWithAIPendingMessagesAsync(1);

            if (result == null || !result.Users.Any())
            {
                return;
            }


            var validUsers = result.Users.Where(w => w.MessagesHistory.Any(f => f.SourceInfo.MessageSource == MessageSource.Discord && f.MessageStatus == MessageStatus.Pending && f.ReferentialType == MessageReferentialType.Assistant));

            DaisyControlMessage messageToSend = validUsers.FirstOrDefault()?.MessagesHistory.FirstOrDefault(f => f.SourceInfo.MessageSource == MessageSource.Discord && f.MessageStatus == MessageStatus.Pending && f.ReferentialType == MessageReferentialType.Assistant);

            if (messageToSend == null)
            {
                return;
            }

            // Reserve that User
            result.Users[0].NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(1);
            if (!await usersHttpClient.UpdateUserAsync(result.Users[0]))
            {
                return;
            }

            result.Users[0].Revision++;

            switch (messageToSend.SourceInfo.MessageSourceType)
            {
                case MessageSourceType.DirectMessage:
                    if (await discordClient.SendDirectMessageAsync(ulong.Parse(messageToSend.SourceInfo.MessageSourceReferential), ulong.Parse(result.Users[0].Id), DaisyControlMessageType.User, messageToSend.MessageContent))
                    {
                        LoggingManager.LogToFile("7ddd855e-3560-48f9-baea-80ea2dae61b4", $"Sent discord DM message to User [{result.Users[0].Id}].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                        messageToSend.MessageStatus = MessageStatus.Processed;
                        result.Users[0].NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;
                        if (!await usersHttpClient.UpdateUserAsync(result.Users[0]))
                        {
                            LoggingManager.LogToFile("b9e1376b-107c-4d41-a2c0-4fa601d54bcd", $"Message was sent to User [{result.Users[0].Id}], but the underlying AI message couldn't be set to Processed. The AI message will be re-sent in duplicate.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                            return;
                        }

                    } else
                    {
                        LoggingManager.LogToFile("b47e8b68-f728-48bd-b093-846f1701e4c2", $"Failed to send discord DM message to User [{result.Users[0].Id}].");
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task HandleMessagesToShowTyping()
        {
            DaisyControlGetUsersResponseDto result = await usersHttpClient.GetWorkingStatusUsersAsync(100);

            if (result == null || !result.Users.Any())
            {
                return;
            }

            var validUsers = result.Users.Where(w => w.Status == UserStatus.Working && w.NextMessageToProcessOperationAvailabilityAtUtc > DateTime.UtcNow && w.MessagesHistory.Any(a => a.MessageStatus == MessageStatus.Pending && a.SourceInfo.MessageSourceType == MessageSourceType.DirectMessage && a.SourceInfo.MessageSource == MessageSource.Discord)).ToArray();

            foreach (DaisyControlUserDto user in validUsers)
            {
                var message = user.MessagesHistory.FirstOrDefault(f => f.SourceInfo.MessageSource == MessageSource.Discord && f.SourceInfo.MessageSourceType == MessageSourceType.DirectMessage && f.MessageStatus == MessageStatus.Pending);

                if (message == null)
                {
                    continue;
                }

                await discordClient.ShowTypingAsync(ulong.Parse(message.SourceInfo.MessageSourceReferential), ulong.Parse(user.Id));
            }
        }
    }
}
