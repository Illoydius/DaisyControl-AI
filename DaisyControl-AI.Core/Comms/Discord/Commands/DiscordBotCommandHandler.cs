using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Core.Comms.Discord.Commands.Executors;
using Discord;
using Discord.WebSocket;

namespace DaisyControl_AI.Core.Comms.Discord.Commands
{
    public class DiscordBotCommandHandler : IDiscordBotCommandHandler
    {
        public async Task HandleNewCommandMessageAsync(SocketUserMessage socketUserMessage, DaisyControlDiscordClient.ReplyToUserCallback replyToUserCallback, DaisyControlDiscordClient.SendChannelMessageCallback sendChannelMessageCallback)
        {
            LoggingManager.LogToFile("1c7232d1-b973-45c7-8ffd-88e01457d817", $"Discord bot received a new COMMAND message: [{socketUserMessage}] from [{socketUserMessage.Author.Username}({socketUserMessage.Author.Id})].", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // TODO: execute command
            string messageValue = socketUserMessage.ToString().ToLowerInvariant().TrimEnd(' ');

            switch (messageValue)
            {
                case "!help":
                    await replyToUserCallback(socketUserMessage.Channel.GetChannelType(), socketUserMessage.Channel.Id, socketUserMessage.Author.Id, DaisyControlMessageType.System, @$"Here's a list of available commands:{Environment.NewLine}{Environment.NewLine}" +
                        $"!help = Return the list of available commands.{Environment.NewLine}{Environment.NewLine}" +
                        $"!set system feedback [0|1] = Set the SYSTEM messages feedback. e.g. \"!set system feedback 1\" to activate them.");
                    break;
                case string messageValueStartWith when messageValueStartWith.StartsWith("!set system feedback"):
                    var result = SystemFeedbackDiscordCommandExecutor.Execute(messageValue, out int resultInt);

                    if (!result)
                    {
                        await replyToUserCallback(socketUserMessage.Channel.GetChannelType(), socketUserMessage.Channel.Id, socketUserMessage.Author.Id, DaisyControlMessageType.System, $"Command [{messageValue}] is badly formatted. {Environment.NewLine}Use \"!help\" command to see a list of available commands.");
                        return;
                    }

                    await replyToUserCallback(socketUserMessage.Channel.GetChannelType(), socketUserMessage.Channel.Id, socketUserMessage.Author.Id, DaisyControlMessageType.System, DiscordMessageUtils.FormatSystemMessage($"The SYSTEM messages are now {(resultInt == 0 ? "disabled" : "enabled")} for your user."));
                    break;
                default:
                    await replyToUserCallback(socketUserMessage.Channel.GetChannelType(), socketUserMessage.Channel.Id, socketUserMessage.Author.Id, DaisyControlMessageType.System, $"Command [{messageValue}] is unhandled. {Environment.NewLine}Use \"!help\" command to see a list of available commands.");
                    break;
            }
        }
    }
}
