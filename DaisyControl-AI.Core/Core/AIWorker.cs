using DaisyControl_AI.Common.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace DaisyControl_AI.Core.Core
{
    public class AIWorker : BackgroundService
    {
        /// <summary>
        /// AI main operative loop.
        /// That's the main 'brain' algorithm where we decide what the AI will focus on next.
        /// We'll then let the AI interact, using its creativity by itself, but we'll steer it before hand.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    // Check if the AI has received new messages from users
                    await HandleNewMessagesFromUsersAsync();

                }
                catch (Exception exception)
                {
                    LoggingManager.LogToFile("129c9062-32de-413f-874b-f86c09eb236a", $"Unhandled exception in {nameof(AIWorker)} main loop.", exception);
                    // TODO: log here
                }

                // TODO: don't wait if we don't need to
                await Task.Delay(1000);
            }
        }

        private async Task HandleNewMessagesFromUsersAsync()
        {
            //var HttpRequestClient = new DaisyControlStorageClient();
            //var usersToProcess = await HttpRequestClient.TryGetUsersWithMessagesToProcessAsync();

            //if (usersToProcess?.Users == null || !usersToProcess.Users.Any())
            //{
            //    return;
            //}

            //foreach (var user in usersToProcess.Users)
            //{
            //    // TODO: process each message, generate the according reply, if required, and send it
            //}
        }
    }
}
