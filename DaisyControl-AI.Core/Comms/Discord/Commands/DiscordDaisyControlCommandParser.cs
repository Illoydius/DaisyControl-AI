using System.Text.RegularExpressions;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    internal static class DiscordDaisyControlCommandParser
    {
        internal static int? GetCommandValue(string messageValue)
        {
            Regex FindLastWordAsNumber = new Regex("(\\d+)(?!.*\\d)");
            var match = FindLastWordAsNumber.Match(messageValue);

            int resultMatchValue = -1;
            if (!match.Success || !int.TryParse(match.Value, out resultMatchValue) || (resultMatchValue != 0 && resultMatchValue != 1))
            {
                return null;
            }

            return resultMatchValue;
        }
    }
}
