using Discord;
using Discord.WebSocket;
using static DaisyControl_AI.Core.Comms.Discord.DaisyControlDiscordClient;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public interface IDiscordBotUserMessageHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketUserMessage"></param>
        /// <param name="replyToUserCallback">The handler can call this delegate to reply to the user instantly, in DMs or in the channel it received the message from.</param>
        /// <returns></returns>
        Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage, ReplyToUserCallback replyToUserCallback);
        Task HandleUpdatedMessageAsync(Cacheable<IMessage, ulong> cahedMessages, IMessage previousMessage, SocketMessage updatedMessage, ISocketMessageChannel channel);
    }
}
