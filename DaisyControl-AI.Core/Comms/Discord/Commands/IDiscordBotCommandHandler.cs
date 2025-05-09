using Discord.WebSocket;
using static DaisyControl_AI.Core.Comms.Discord.DaisyControlDiscordClient;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public interface IDiscordBotCommandHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketUserMessage"></param>
        /// <param name="replyToUserCallback">The handler can call this delegate to reply to the user instantly, in DMs or in the channel it received the message from.</param>
        /// <returns></returns>
        Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage, ReplyToUserCallback replyToUserCallback1);
    }
}
