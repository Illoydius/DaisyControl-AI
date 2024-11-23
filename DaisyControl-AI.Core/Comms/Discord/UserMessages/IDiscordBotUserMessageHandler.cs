using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public interface IDiscordBotUserMessageHandler
    {
        Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage);
    }
}
