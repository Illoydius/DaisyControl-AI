using DaisyControl_AI.Common.Configuration.Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static DaisyControl_AI.Common.Diagnostics.LoggingManager;

namespace DaisyControl_AI.Common.Configuration
{
    /// <summary>
    /// Global configuration for DaisyControl.
    /// </summary>
    public class DaisyControlConfig
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DiscordBotConfiguration DiscordBotConfiguration { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogVerbosity LogVerbosity { get; set; } = LogVerbosity.Minimal;
    }
}
