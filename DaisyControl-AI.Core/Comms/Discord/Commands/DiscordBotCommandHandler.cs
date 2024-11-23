using DaisyControl_AI.Common.Diagnostics;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public class DiscordBotCommandHandler : IDiscordBotCommandHandler
    {
        public async Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage)
        {
            LoggingManager.LogToFile("1c7232d1-b973-45c7-8ffd-88e01457d817", $"Discord bot received a new COMMAND message: [{socketUserMessage}] from [{socketUserMessage.Author.Username}({socketUserMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // TODO: execute command
        }
    }
}
