using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Core.Core.Decisions.Schedule
{
    internal static class ScheduleManager
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();

        /// <summary>
        /// If Daisy has a pending message from at least one User, we want to determine WHEN she'll answer those message
        /// we need to check if she'll do it immediately or if she'll wait for a hole in her schedule before answering (depending on her mood/short term goals, etc)
        /// </summary>
        public static async Task<bool> ReflectOnScheduleToCheckNewUsersMessages(DaisyControlUserDto userToProcess)
        {
            if (userToProcess == null)
            {
                return false;
            }

            userToProcess.LastThoughtAboutAtUtc = DateTime.UtcNow;
            userToProcess.Status = UserStatus.Ready;
            await usersHttpClient.UpdateUserAsync(userToProcess);

            // First thing is to check if Daisy has the time to check for new messages on her phone. To achieve this, we're simulating a Schedule for her
            // Check what's she's doing now and what she has to do in the next X mins to see if she has the time to check her phone (full on roleplay ^^)

            // To achieve this, we'll get all users with pending User Messages and check if the NextMessageToProcessOperationAvailabilityAtUtc is >= 1 month (since we set MaxValue when we receive a new message)
            // TODO

            // Then, for each of those user, we'll query the LLM with the relevant context (schedule) to see if they want to check the new message or wait for a more opportune time
            // TODO

            // Finally, we'll set the NextMessageToProcessOperationAvailabilityAtUtc to the computed value determined by the LLM
            // TODO

            return true;
        }

        /// <summary>
        /// Return the timespan that Daisy can use before she needs to do something else (like work, cook, etc)
        /// </summary>
        /// <returns>TimeSpan we can use before Daisy has to do something else.</returns>
        public static async Task<bool> GetFreeTimeSpanBeforeNextPrioritaryScheduleItem()
        {
            // Start by checking Daisy Schedule to see what she's doing right now

            //var daisyMind = DaisyMindFactory.GenerateDaisyMind(

            // TODO: then check what she needs to do next (and WHEN)
            return true;
        }
    }
}
