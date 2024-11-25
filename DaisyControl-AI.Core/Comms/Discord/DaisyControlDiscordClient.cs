using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Core.Comms.Discord.Commands;
using DaisyControl_AI.Core.Comms.Discord.UserMessages;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord
{
    public class DaisyControlDiscordClient : IDaisyControlDiscordClient
    {
        private static DiscordSocketClient discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
        {
            MessageCacheSize = 1024,
        });

        bool isReady = false;// When the websocket is bound
        string fDiscordBotToken = null;
        IDiscordBotCommandHandler discordBotCommandHandler;
        IDiscordBotUserMessageHandler discordBotUserMessageHandler;
        CommandService fDiscordBotCommandService = new CommandService();
        public delegate Task<bool> ReplyToUserCallback(ChannelType? channelType, ulong channelId, ulong userId, DaisyControlMessageType messageType, string message);
        ReplyToUserCallback replyToUserCallback;
        public delegate Task<bool> SendChannelMessageCallback(ulong channelId, DaisyControlMessageType messageType, string message);
        SendChannelMessageCallback sendChannelMessageCallback;

        public DaisyControlDiscordClient(
            IDiscordBotCommandHandler discordBotCommandHandler,
            IDiscordBotUserMessageHandler discordBotUserMessageHandler)
        {
            discordSocketClient.Log += LogDiscordClientEvent;
            fDiscordBotCommandService.Log += LogDiscordCommandEvent;

            fDiscordBotToken = CommonConfigurationManager.ReloadConfig()?.DiscordBotConfiguration?.SecretToken;

            if (string.IsNullOrWhiteSpace(fDiscordBotToken))
            {
                LoggingManager.LogToFile("35408ede-85fc-4e43-be33-b6cfe47eefd0", $"Invalid Discord Bot Token [{fDiscordBotToken}]. Please check your config.json. Fix your config file and restart the server.");

                return;
            }

            replyToUserCallback = ReplyWithMessageAsync;
            sendChannelMessageCallback = SendMessageAsync;

            this.discordBotCommandHandler = discordBotCommandHandler;
            this.discordBotUserMessageHandler = discordBotUserMessageHandler;

            discordSocketClient.MessageUpdated += MessageUpdated;
            discordSocketClient.MessageReceived += MessageReceived;
            discordSocketClient.Ready += () =>
            {
                isReady = true;
                LoggingManager.LogToFile("66a8d6b1-f8ee-4719-bd51-8db3973aaade", $"Discord bot is connected.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return Task.CompletedTask;
            };

            // Bind discord to a specific socket
            BindDiscordBot().Wait();
        }

        /// <summary>
        /// Stop the connection with Discord Gateway.
        /// </summary>
        public async Task StopAsync()
        {
            await discordSocketClient?.StopAsync();
        }

        private async Task BindDiscordBot()
        {
            await discordSocketClient.LoginAsync(TokenType.Bot, fDiscordBotToken);
            await discordSocketClient.StartAsync();
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
                await discordBotCommandHandler.HandleNewCommandMessageAsync(socketUserMessage, replyToUserCallback, sendChannelMessageCallback);
                return;
            }

            await discordBotUserMessageHandler.HandleNewClientMessageAsync(socketUserMessage, replyToUserCallback, sendChannelMessageCallback);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> cahedMessages, SocketMessage updatedMessage, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var previousMessage = await cahedMessages.GetOrDownloadAsync();

            await discordBotUserMessageHandler.HandleUpdatedMessageAsync(cahedMessages, previousMessage, updatedMessage, channel);
        }

        public async Task<bool> SendMessageAsync(ulong channelId, DaisyControlMessageType messageType, string message)
        {
            var channel = await discordSocketClient.GetChannelAsync(channelId) as SocketTextChannel;

            if (channel == null)
            {
                LoggingManager.LogToFile("7cb05d30-8ad1-4459-8596-bfe39e2ee97d", $"Couldn't send message [{message}] from bot to channelId [{channelId}]. Channel couldn't be found.");
                return false;
            }

            LoggingManager.LogToFile("d4433dcc-e219-435a-86f7-e613e96476bd", $"AI sent the following channel message ([{messageType}]) to channelId [{channelId}] : [{message}].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            var response = await channel.SendMessageAsync(message).ConfigureAwait(false);
            return response != null;
        }

        public async Task<bool> SendDirectMessageAsync(ulong dmChannelId, ulong userId, DaisyControlMessageType messageType, string message)
        {
            var channel = await discordSocketClient.GetDMChannelAsync(dmChannelId);

            if (channel == null)
            {
                LoggingManager.LogToFile("c6b25136-57bb-40c0-b997-f8bf7c16c2a6", $"Couldn't send message [{message}] from bot to user [{userId}] in DM channelId [{dmChannelId}]. User couldn't be found.");
                return false;
            }

            LoggingManager.LogToFile("66c2a1c0-118d-487e-b662-29b8f322bcd3", $"AI sent the following direct message ([{messageType}]) to userId [{userId}] in DM channelId [{dmChannelId}] : [{message}].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            var response = await channel.SendMessageAsync(message);

            return response != null;
        }

        public async Task<bool> ReplyWithMessageAsync(ChannelType? channelType, ulong channelId, ulong userId, DaisyControlMessageType messageType, string message)
        {
            // Determine if we need to reply using a DM or a ChannelMessage
            switch (channelType)
            {
                case ChannelType.Text:
                    return await SendMessageAsync(channelId, messageType, message);
                case ChannelType.DM:
                    return await SendDirectMessageAsync(channelId, userId, messageType, message);
                case ChannelType.Voice:
                case ChannelType.Group:
                case ChannelType.Category:
                case ChannelType.News:
                case ChannelType.Store:
                case ChannelType.NewsThread:
                case ChannelType.PublicThread:
                case ChannelType.PrivateThread:
                case ChannelType.Stage:
                case ChannelType.GuildDirectory:
                case ChannelType.Forum:
                case ChannelType.Media:
                case null:
                default:
                    LoggingManager.LogToFile("1ada08d5-2ea5-4142-a5ef-b2ada931fd57", $"Couldn't send message [{message}] from bot to user [{userId}]. ChannelType [{channelType}] is unhandled.");
                    return false;
            }
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
            {
                Console.WriteLine($"{prefix}[General_{discordLogMessage.Severity}] {discordLogMessage}");
            }

            return Task.CompletedTask;
        }

        public bool IsReady => isReady;
    }
}
