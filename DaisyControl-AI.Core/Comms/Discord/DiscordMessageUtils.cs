namespace DaisyControl_AI.Core.Comms.Discord
{
    internal static class DiscordMessageUtils
    {
        internal static string FormatSystemMessage(string rawMessage)
        {
            return $"[SYSTEM] - {rawMessage}";
        }
    }
}
