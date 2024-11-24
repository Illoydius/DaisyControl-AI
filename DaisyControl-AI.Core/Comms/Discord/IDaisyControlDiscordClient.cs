using Discord;

namespace DaisyControl_AI.Core.Comms.Discord
{
    public interface IDaisyControlDiscordClient
    {
        Task StopAsync();
        Task ReplyWithMessageAsync(ChannelType? channelType, ulong channelId, ulong userId, DaisyControlMessageType messageType, string message);// Reply to a user, using automatically either a DM or a channelMessage, depending on the context
        Task SendMessageAsync(ulong channelId, DaisyControlMessageType messageType, string message);
        Task SendDirectMessageAsync(ulong dmChannelId, ulong userId, DaisyControlMessageType messageType, string message);
        bool IsReady { get; }
    }
}
