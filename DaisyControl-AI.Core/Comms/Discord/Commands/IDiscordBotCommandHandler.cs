using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public interface IDiscordBotCommandHandler
    {
        Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage);
    }
}
