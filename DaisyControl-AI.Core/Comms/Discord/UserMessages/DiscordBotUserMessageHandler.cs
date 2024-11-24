using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos.Response;
using Discord;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public class DiscordBotUserMessageHandler : IDiscordBotUserMessageHandler
    {
        public async Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage)
        {
            if (socketUserMessage?.Author == null)
            {
                LoggingManager.LogToFile("ec9c3c64-8abc-4b94-8a65-14ee47d44f62", $"Discord bot received a new invalid message: [{socketUserMessage}]. Full message object=[{JsonSerializer.Serialize(socketUserMessage)}].");
                return;
            }

            LoggingManager.LogToFile("73d4fa09-9410-4408-93e8-921570260372", $"Discord bot received a new message: [{socketUserMessage}] from [{socketUserMessage.Author.Username}({socketUserMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // Execute message handling
            // Here, we have a few main options. If it's a new user, we'll go to the Onboarding Action, which will make the AI talk to the new user and eventually ask the user to register (create a new User in Storage)
            // Or, if it's an existing user, we'll go in the Core (main) Action, that will make the AI fetch the data we have on that user and see how to proplery respond to the message.

            // Get User if he wasn't provided
            var HttpRequestClient = new DaisyControlStorageClient();
            DaisyControlUserResponseDto user = await HttpRequestClient.GetUserAsync(socketUserMessage.Author.Id).ConfigureAwait(false);

            if (user == null)
            {
                // Create User in DB, even if we only know his ID and Username
                user = await HttpRequestClient.AddUserAsync(socketUserMessage.Author.Id, socketUserMessage.Author.Username).ConfigureAwait(false);
            }

            // Next, we want to create the AI "DaisyMind" related to that User. (A User could personalize the bot personality, for instance, so it's unique for each user)
            DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(user).ConfigureAwait(false);

            if (daisyMind.DaisyMemory.User?.CharacterSheet?.FirstName == null)
            {
                // New user, go to onboarding handling
                // We'll add a new LifeEvent here, something like "XXX just sent you the following text message "YYY", build a text message reply, you can use smileys. ...

            } else
            {
                // The user is known, go to the main handling

                // TODO
            }
        }

        public async Task HandleUpdatedMessageAsync(Cacheable<IMessage, ulong> cahedMessages, IMessage previousMessage, SocketMessage updatedMessage, ISocketMessageChannel channel)
        {
            LoggingManager.LogToFile("70260d8b-1d35-491b-b8ea-8d5e95a253d8", $"Discord bot received a notification that the message [{previousMessage}] was updated to [{updatedMessage}] from [{updatedMessage.Author.Username}({updatedMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // TODO: handle message update... maybe the AI has something to say about it ? ;)
        }
    }
}
