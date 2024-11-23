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
        /// <summary>
        /// Discord bot TOKEN from developper console. Do not share this field value with others.
        /// </summary>
        public string DiscordBotSecretToken { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogVerbosity LogVerbosity { get; set; } = LogVerbosity.Minimal;
    }
}
