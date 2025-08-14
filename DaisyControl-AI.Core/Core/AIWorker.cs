using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.Core.Decisions.FollowUp;
using DaisyControl_AI.Core.Core.Decisions.Goals;
using DaisyControl_AI.Core.Core.Decisions.Schedule;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;
using Microsoft.Extensions.Hosting;

namespace DaisyControl_AI.Core.Core
{
    public class AIWorker : BackgroundService
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();

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
                    // Refresh the AI 'surface' thoughts
                    await ReflectOnSelf();

                } catch (Exception exception)
                {
                    LoggingManager.LogToFile("129c9062-32de-413f-874b-f86c09eb236a", $"Unhandled exception in {nameof(AIWorker)} main loop.", exception);
                }

                // TODO: don't wait if we don't need to
                await Task.Delay(3000);
            }
        }

        private async Task ReflectOnSelf()
        {
            // Start by thinking about the User that Daisy has not thought about for the longest
            DaisyControlGetUsersResponseDto usersDto = await usersHttpClient.GetUsersWithOldestThinkRefreshTimeAsync(1);

            if (usersDto == null)
            {
                return;
            }

            DaisyControlUserDto userToProcess = null;
            foreach (DaisyControlUserDto user in usersDto.Users)
            {
                user.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(30);
                user.Status = UserStatus.Working;
                if (await usersHttpClient.UpdateUserAsync(user))
                {
                    ++user.Revision;
                    userToProcess = user;
                    break;
                }
            }

            if (userToProcess == null)
            {
                return;
            }

            // ------------------------------------------------------------------------------------------------------------
            // If Daisy has a pending message from at least one User, we want to determine WHEN she'll answer those message
            // we need to check if she'll do it immediately or if she'll wait for a hole in her schedule before answering (depending on her mood/short term goals, etc)
            if(await ScheduleManager.ReflectOnScheduleToCheckNewUsersMessages(userToProcess))
                return;

            // TODO: the return of ReflectOnScheduleToCheckNewUsersMessages should tell us if Daisy has the time to spend on that user. Maybe she's working right now or something..

            // Check for goals for each user Daisy knows (ex: enhance knowledge (name, age, career, hobbies,etc), give task, etc)
            // TODO: check schedule. It should be done BEFORE deciding to answer a User, but not every 3 seconds ..
            //if(await GoalsDecisionManager.ReflectOnImmediateGoalsForNextAvailableUser(userToProcess))
                //return;

            // ------------------------------------------------------------------------------------------------------------
            // If it's been ~45 sec since the AI sent a message and there's no come back from User, sent an inference task to check if it would make sense to send another message. A follow up of sort.
            // TODO: check schedule. If Daisy doesn't have the time to follow-up, don't even bother to generete it
            //if(await FollowUpManager.ReflectOnFollowUpConversations(userToProcess))
              //  return;

            // ------------------------------------------------------------------------------------------------------------
            // TODO: handle AI schedule (what she's doing today, take a bath = have some time to chat, etc)

            // ------------------------------------------------------------------------------------------------------------
            // TODO: Check for goals around itself (ex: career, holidays, new clothes, new phone, new car, etc)

            // ------------------------------------------------------------------------------------------------------------
            // TODO: Check for long-term goals for each user it knows(ex: change the relationship dynamic, set new rules, etc)
        }
    }
}
