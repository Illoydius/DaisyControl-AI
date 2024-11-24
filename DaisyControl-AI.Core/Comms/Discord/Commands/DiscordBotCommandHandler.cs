using DaisyControl_AI.Common.Diagnostics;
using Discord;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public class DiscordBotCommandHandler : IDiscordBotCommandHandler
    {
        public async Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage, DaisyControlDiscordClient.ReplyToUserCallback replyToUserCallback, DaisyControlDiscordClient.SendChannelMessageCallback sendChannelMessageCallback)
        {
            LoggingManager.LogToFile("1c7232d1-b973-45c7-8ffd-88e01457d817", $"Discord bot received a new COMMAND message: [{socketUserMessage}] from [{socketUserMessage.Author.Username}({socketUserMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // TODO: execute command
            string messageValue = socketUserMessage.ToString().ToLowerInvariant();

            switch (messageValue)
            {
                case "!help":
                    break;
                default:
                    await replyToUserCallback(socketUserMessage.Channel.GetChannelType(), socketUserMessage.Channel.Id, socketUserMessage.Author.Id, DaisyControlMessageType.System, $"Command [{messageValue}] is unhandled. {Environment.NewLine}Use \"!help\" command to see a list of available commands.");
                    break;
            }
        }
    }
}
