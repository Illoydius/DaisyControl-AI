using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;
using Discord;
using Discord.WebSocket;
using static DaisyControl_AI.Core.Comms.Discord.DaisyControlDiscordClient;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public class DiscordBotUserMessageHandler : IDiscordBotUserMessageHandler
    {
        DaisyControlStorageUsersClient httpRequestClient = new();
        //string messagesUrl = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/messagesbuffer";
        //string usersUrl = $"{DaisyControlConstants.StorageWebApiBaseUrl}api/users";

        public async Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage, ReplyToUserCallback replyToUserCallback)
        {
            if (socketUserMessage?.Author == null)
            {
                LoggingManager.LogToFile("ec9c3c64-8abc-4b94-8a65-14ee47d44f62", $"Discord bot received a new invalid message: [{socketUserMessage}]. Full message object=[{JsonSerializer.Serialize(socketUserMessage)}].");
                return;
            }

            LoggingManager.LogToFile("73d4fa09-9410-4408-93e8-921570260372", $"Discord bot received a new message: [{socketUserMessage}] from [{socketUserMessage.Author.Username}({socketUserMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // Get the user from storage
            DaisyControlUserDto user = null;

            try
            {
                user = await httpRequestClient.GetUserAsync(socketUserMessage.Author.Id).ConfigureAwait(false);
            } catch (Exception exception)
            {
                LoggingManager.LogToFile("2046bd53-aae0-439f-8246-f14e07e4d33d", $"Discord bot couldn't find user [{socketUserMessage.Author.Id}]. The message from this user will be ignored.", aException: exception);
                return;
            }

            if (user == null)
            {
                // Create User in DB, even if we only know his ID and Username
                user = await httpRequestClient.AddUserAsync(socketUserMessage.Author.Id, socketUserMessage.Author.GlobalName).ConfigureAwait(false);

                if (user == null)
                {
                    LoggingManager.LogToFile("84f8333a-35a4-4ac2-8ec3-172018e0f2a5", $"Discord bot couldn't create user [{socketUserMessage.Author.Id}]. The message from this user will be ignored.");
                    return;
                }
            }

            // Next, we want to create the AI "DaisyMind" related to that User. (A User could personalize the bot personality, for instance, so it's unique for each user)
            DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(user).ConfigureAwait(false);

            // Add the new user message to DaisyMind memory
            var userMessage = new DaisyControlMessage
            {
                CreatedAtUtc = DateTime.UtcNow,
                ReferentialType = MessageReferentialType.User,
                MessageContent = $"{socketUserMessage.ToString()}",
                MessageStatus = MessageStatus.Pending,
                SourceInfo = new()
                {
                    MessageSource = Storage.Dtos.MessageSource.Discord,
                    MessageSourceType = socketUserMessage.Channel.GetType() == typeof(SocketDMChannel) ? MessageSourceType.DirectMessage : MessageSourceType.Channel,
                    MessageSourceReferential = socketUserMessage.Channel.Id.ToString(),
                },
            };

            // Response was correctly received from user. Save the user message to storage
            daisyMind.DaisyMemory.User.Global.MessagesHistory ??= new();
            daisyMind.DaisyMemory.User.Global.MessagesHistory.Add(userMessage);
            daisyMind.DaisyMemory.User.Global.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow;

            int retryIterator = 0;
            while (true)
            {
                bool storageUpdateSuccess = await httpRequestClient.UpdateUserAsync(daisyMind.DaisyMemory.User.Global).ConfigureAwait(false);

                if (!storageUpdateSuccess)
                {
                    ++retryIterator;

                    if (retryIterator >= 300)
                    {
                        LoggingManager.LogToFile("62ae5779-84b1-4127-b868-22d097d3273a", $"Message from user [{socketUserMessage.Author.Id}] was'nt registered properly in storage. The AI will ignore this message from the User [{socketUserMessage.Author.Username}].");
                        return;
                    }

                    await Task.Delay(1000);
                } else
                {
                    LoggingManager.LogToFile("27d04e3a-697b-42d2-835f-f2ff82bf0dbe", $"Message from user [{socketUserMessage.Author.Id}] was properly registered and queued to be processed by the AI as soon as possible.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                    break;
                }
            }
        }

        public async Task HandleUpdatedMessageAsync(Cacheable<IMessage, ulong> cahedMessages, IMessage previousMessage, SocketMessage updatedMessage, ISocketMessageChannel channel)
        {
            LoggingManager.LogToFile("70260d8b-1d35-491b-b8ea-8d5e95a253d8", $"Discord bot received a notification that the message [{previousMessage}] was updated to [{updatedMessage}] from [{updatedMessage.Author.Username}({updatedMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // TODO: handle message update... maybe the AI has something to say about it ? ;)
        }
    }
}
