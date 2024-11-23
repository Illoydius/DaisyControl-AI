using System.Diagnostics.Metrics;
using System.Windows.Input;
using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Core.Comms.Discord.Commands;
using DaisyControl_AI.Core.Comms.Discord.UserMessages;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord
{
    public class DiscordWorker
    {
        private static DiscordSocketClient fDiscordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
        {
            MessageCacheSize = 1024,
        });

        IDiscordBotCommandHandler discordBotCommandHandler;
        IDiscordBotUserMessageHandler discordBotUserMessageHandler;
        CommandService fDiscordBotCommandService = new CommandService();
        string fDiscordBotToken = null;
        bool fIsReady = false;// When the bot is ready to work

        public DiscordWorker(
            IDiscordBotCommandHandler discordBotCommandHandler,
            IDiscordBotUserMessageHandler discordBotUserMessageHandler)
        {
            this.discordBotCommandHandler = discordBotCommandHandler;
            this.discordBotUserMessageHandler = discordBotUserMessageHandler;

            fDiscordSocketClient.Log += LogDiscordClientEvent;
            fDiscordBotCommandService.Log += LogDiscordCommandEvent;

            fDiscordBotToken = CommonConfigurationManager.ReloadConfig()?.DiscordBotSecretToken;

            if (string.IsNullOrWhiteSpace(fDiscordBotToken))
            {
                LoggingManager.LogToFile("35408ede-85fc-4e43-be33-b6cfe47eefd0", $"Invalid Discord Bot Token [{fDiscordBotToken}]. Please check your config.json. Fix your config file and restart the software.");

                return;
            }

            fDiscordSocketClient.MessageUpdated += MessageUpdated;
            fDiscordSocketClient.MessageReceived += MessageReceived;
            fDiscordSocketClient.Ready += () =>
            {
                fIsReady = true;
                LoggingManager.LogToFile("66a8d6b1-f8ee-4719-bd51-8db3973aaade", $"Discord bot is connected.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return Task.CompletedTask;
            };

            // Bind discord to a specific socket
            BindDiscordBot().Wait();

            // Start the worker
            Task.Run(Loop);
        }

        private async Task BindDiscordBot()
        {
            await fDiscordSocketClient.LoginAsync(TokenType.Bot, fDiscordBotToken);
            await fDiscordSocketClient.StartAsync();
        }

        /// <summary>
        /// Worker main operative loop.
        /// </summary>
        private async Task Loop()
        {
            while (true)
            {
                try
                {
                    if (fIsReady)
                    {
                        BackgroundWork();
                    }
                } catch (Exception exception)
                {
                    LoggingManager.LogToFile("8708b99a-0030-419f-9f82-d65c185be004", $"Unhandled exception in discord bot worker main loop.", exception);
                    // TODO: log here
                }

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Stop the connection with Discord Gateway.
        /// </summary>
        public async Task StopAsync()
        {
            await fDiscordSocketClient?.StopAsync();
        }

        /// <summary>
        /// Background thread running concurrently to handle backend processes.
        /// </summary>
        private void BackgroundWork()
        {
            // TODO in another class
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message == null || message.Author.IsBot)
            {
                return;
            }

            if (message is not SocketUserMessage socketUserMessage)
            {
                LoggingManager.LogToFile("b418adb9-d7e9-4f29-835c-49e5e5a89138", $"Discord bot received a new unhandled message: [{message}] from [{message.Author.Username}({message.Author.Id})]. The message was of type [{message.GetType()}], which is unhandled.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            string messageValue = message.ToString();
            if (messageValue.Length > 1 && messageValue.First() == '!')
            {
                await discordBotCommandHandler.HandleNewCommandMessageAsync(socketUserMessage);
                return;
            }

            await discordBotUserMessageHandler.HandleNewClientMessageAsync(socketUserMessage);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> cahedMessages, SocketMessage updatedMessage, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var previousMessage = await cahedMessages.GetOrDownloadAsync();

            LoggingManager.LogToFile("70260d8b-1d35-491b-b8ea-8d5e95a253d8", $"Discord bot received a notification that the message [{previousMessage}] was updated to [{updatedMessage}] from [{updatedMessage.Author.Username}({updatedMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        private static Task LogDiscordClientEvent(LogMessage discordLogMessage) => LogDiscordEvent(discordLogMessage, "[Discord]-");
        private static Task LogDiscordCommandEvent(LogMessage discordLogMessage) => LogDiscordEvent(discordLogMessage, "[Discord]-");

        private static Task LogDiscordEvent(LogMessage discordLogMessage, string prefix = "")
        {
            if (discordLogMessage.Exception is CommandException cmdException)
            {
                Console.WriteLine($"{prefix}[Command_{discordLogMessage.Severity}] {cmdException.Command.Aliases.FirstOrDefault()} failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            } else
                Console.WriteLine($"{prefix}[General_{discordLogMessage.Severity}] {discordLogMessage}");

            return Task.CompletedTask;
        }
    }
}
