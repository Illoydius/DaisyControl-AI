namespace DaisyControl_AI.Common.Configuration.Discord
{
    public class DiscordBotConfiguration
    {
        /// <summary>
        /// Enable or disable the feature.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Discord bot TOKEN from developper console. Do not share this field value with others.
        /// </summary>
        public string SecretToken { get; set; }

        /// <summary>
        /// Configuration related to the SYSTEM feature of the discord bot.
        /// </summary>
        public DiscordBotSystemConfiguration SystemLogs { get; set; }
    }
}
