using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Core.InferenceServer;
using DaisyControl_AI.Core.InferenceServer.Context;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Core.Core.Decisions.FollowUp
{
    // TODO: if the AI sends multiple messages and the user doesn't answer, add a semi-persistent mood (vexed/annoyed) for a few days
    internal static class FollowUpManager
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();
        private static Random random = new Random(DateTime.Now.Millisecond);

        public static async Task<bool> ReflectOnFollowUpConversations(DaisyControlUserDto userToProcessTODO)
        {
            // TODO: generate possibility of new FollowUpRequests instead of getting users with existing requests
            DaisyControlGetUsersResponseDto usersDto = await usersHttpClient.GetUsersWithFollowUpRequestsAsync(1);

            if (usersDto == null)
            {
                return false;
            }

            DaisyControlUserDto userToProcess = null;
            foreach (DaisyControlUserDto user in usersDto.Users)
            {
                user.NextFollowUpAvailabilityAtUtc = DateTime.UtcNow.AddSeconds(150);// 2m30s
                user.Status = Storage.Dtos.UserStatus.Working;
                if (await usersHttpClient.UpdateUserAsync(user))
                {
                    ++user.Revision;
                    userToProcess = user;
                    break;
                }
            }

            if (userToProcess == null)
            {
                return false;
            }

            bool requireFollowUp = false;

            try
            {
                var mostRecentMessageDateTime = userToProcess.MessagesHistory.Max(m => m.CreatedAtUtc);

                // If the last message is still fairly new
                if (mostRecentMessageDateTime.TotalOffsetMinutes <= 15)
                {
                    // Query AI with the context and ask it if it wants to add a follow-up message
                    DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(userToProcess).ConfigureAwait(false);
                    string context = AskForFollowUpContextBuilder.BuildContext(daisyMind, userToProcess);
                    InferenceServerPromptResultResponseDto AIresponse = await InferenceServerQueryer.GenerateStandardAiResponseAsync(context).ConfigureAwait(false);

                    // TODO: Add a floor 5% chance that the AI will follow-up no matter what
                    requireFollowUp = AIresponse.Text.ToLowerInvariant().Contains("true") || AIresponse.Text.ToLowerInvariant().Contains("yes");

                    if (requireFollowUp)
                    {
                        // TODO: Poke AI to generate new message to send to User
                    }
                    else
                    {
                        // TODO: handle sleeping schedule to avoid sending message whilst the user is sleeping
                        userToProcess.NextFollowUpAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(random.Next(120, 2280));// between 2 hours and 2 days, the AI will poke the User again
                    }
                }
                else
                {
                    // TODO: We're not in a living conversation follow-up scenario here. The conversation ended 'a while ago' and we want the AI to poke the User to start a brand new conversation
                }
            }
            finally
            {
                userToProcess.Status = Storage.Dtos.UserStatus.Ready;
                if (!await usersHttpClient.UpdateUserAsync(userToProcess))
                {
                    LoggingManager.LogToFile("3fab84cc-f7b8-47d8-a5fe-b6383736f90a", $"Conversation with user [{userToProcess.Id}({userToProcess.UserInfo.Username})] follow-up update failed. Couldn't save the follow-up request to [{userToProcess.NextFollowUpAvailabilityAtUtc}].");
                }
            }

            return false;
        }
    }
}
