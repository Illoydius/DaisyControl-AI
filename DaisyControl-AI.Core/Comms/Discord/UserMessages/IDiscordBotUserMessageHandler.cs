﻿using Discord;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.UserMessages
{
    public interface IDiscordBotUserMessageHandler
    {
        Task HandleNewClientMessageAsync(SocketUserMessage socketUserMessage);
        Task HandleUpdatedMessageAsync(Cacheable<IMessage, ulong> cahedMessages, IMessage previousMessage, SocketMessage updatedMessage, ISocketMessageChannel channel);
    }
}