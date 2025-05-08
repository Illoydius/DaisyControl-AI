using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace DaisyControl_AI.Core.Comms.Discord
{
    /// <summary>
    /// Background worker on Discord interactions.
    /// Handles AI communications From/To discord.
    /// TODO: handle files(pictures, txt, etc).
    /// </summary>
    public class DiscordWorker : BackgroundService
    {
        private readonly IDaisyControlDiscordClient discordClient;

        public DiscordWorker(
            IDaisyControlDiscordClient discordClient)// The discordClient Ctor will auto subscribe on new messages
        {
            this.discordClient = discordClient;
        }

        /// <summary>
        /// Worker main operative loop.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = CommonConfigurationManager.ReloadConfig();
            if(config?.DiscordBotConfiguration?.Enabled == null || !config.DiscordBotConfiguration.Enabled)
            {
                return;
            }

            while (true)
            {
                try
                {
                    if (discordClient != null && discordClient.IsConnected)
                    {
                        BackgroundWork();
                    }
                } catch (Exception exception)
                {
                    LoggingManager.LogToFile("8708b99a-0030-419f-9f82-d65c185be004", $"Unhandled exception in discord bot worker main loop.", exception);
                }

                // TODO: don't wait if we don't need to
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Stop the connection with Discord Gateway.
        /// </summary>
        public async Task StopAsync()
        {
            await discordClient?.StopAsync();
        }

        /// <summary>
        /// Background thread running concurrently to handle backend processes.
        /// </summary>
        private void BackgroundWork()
        {
            // TODO in another class
        }
    }
}
