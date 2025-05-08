using Discord;

namespace DaisyControl_AI.Core.Comms.Discord
{
    public interface IDaisyControlDiscordClient
    {
        Task StopAsync();
        Task<bool> ReplyWithMessageAsync(ChannelType? channelType, ulong channelId, ulong userId, DaisyControlMessageType messageType, string message);// Reply to a user, using automatically either a DM or a channelMessage, depending on the context
        Task<bool> SendMessageAsync(ulong channelId, DaisyControlMessageType messageType, string message);
        Task<bool> SendDirectMessageAsync(ulong dmChannelId, ulong userId, DaisyControlMessageType messageType, string message);
        bool IsConnected { get; }
    }
}
