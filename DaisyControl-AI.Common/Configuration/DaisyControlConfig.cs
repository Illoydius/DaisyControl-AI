using System.Text.Json.Serialization;
using DaisyControl_AI.Common.Configuration.Discord;
using DaisyControl_AI.Common.Configuration.Storage;
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
        [JsonPropertyName("discordBotConfiguration")]
        public DiscordBotConfiguration DiscordBotConfiguration { get; set; }

        [JsonPropertyName("storageConfiguration")]
        public StorageConfiguration StorageConfiguration { get; set; }

        [JsonPropertyName("inferenceNodeConfiguration")]
        public InferenceNodeConfiguration InferenceNodeConfiguration { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("logVerbosity")]
        public LogVerbosity LogVerbosity { get; set; } = LogVerbosity.Minimal;
    }
}
