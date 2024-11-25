using Discord.WebSocket;
using static DaisyControl_AI.Core.Comms.Discord.DaisyControlDiscordClient;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public interface IDiscordBotCommandHandler
    {
        Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage, ReplyToUserCallback replyToUserCallback1, SendChannelMessageCallback sendChannelMessageCallback);
    }
}
