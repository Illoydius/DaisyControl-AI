using Discord;
using Discord.WebSocket;
using static DaisyControl_AI.Core.Comms.Discord.DaisyControlDiscordClient;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public interface IDiscordBotUserMessageHandler
    {
        Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage, ReplyToUserCallback replyToUserCallback, SendChannelMessageCallback sendChannelMessageCallback);
        Task HandleUpdatedMessageAsync(Cacheable<IMessage, ulong> cahedMessages, IMessage previousMessage, SocketMessage updatedMessage, ISocketMessageChannel channel);
    }
}
