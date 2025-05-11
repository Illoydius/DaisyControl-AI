using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Core.Core.Decisions.Goals;
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
                    // Refresh the AI goals
                    await ReflectOnSelfGoals();

                } catch (Exception exception)
                {
                    LoggingManager.LogToFile("129c9062-32de-413f-874b-f86c09eb236a", $"Unhandled exception in {nameof(AIWorker)} main loop.", exception);
                    // TODO: log here
                }

                // TODO: don't wait if we don't need to
                await Task.Delay(3000);
            }
        }

        private async Task ReflectOnSelfGoals()
        {
            // TODO: Check for its next goal for each user it knows (ex: enhance knowledge (name, age, career, hobbies,etc), give task, etc)
            if (await GoalsDecisionManager.ReflectOnImmediateGoalsForNextAvailableUser())
            {
                return;

                // TODO: Check for goals around itself (ex: career, holidays, new clothes, new phone, new car, etc)

                // TODO: Check for long-term goals for each user it knows(ex: change the relationship dynamic, set new rules, etc)
            }

            // TODO: If it's been like ~45 sec since the AI sent a message and there's no come back from User, sent an inference task to check if it would make sense to send another message. A follow up of sort.
        }
    }
}
