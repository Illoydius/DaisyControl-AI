using DaisyControl_AI.Common.Diagnostics;

namespace DaisyControl_AI.Core.Comms.Discord.Commands.Executors
{
    public static class SystemFeedbackDiscordCommandExecutor
    {
        public static bool Execute(string messageValue, out int resultInt)
        {
            resultInt = -1;
            int? resultValueCommandParser = DiscordDaisyControlCommandParser.GetCommandValue(messageValue);

            if (resultValueCommandParser == null)
            {
                LoggingManager.LogToFile("1037e701-d96a-4ccb-8573-dda815876a39", $"Couldn't find command value in [{messageValue}].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return false;
            }

            resultInt = resultValueCommandParser.Value;

            // TODO: enable or disable the setting for this user
            return true;
        }
    }
}
