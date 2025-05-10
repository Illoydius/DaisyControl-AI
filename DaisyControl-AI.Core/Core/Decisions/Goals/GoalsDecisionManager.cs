using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;
using Discord;
using UserStatus = DaisyControl_AI.Storage.Dtos.UserStatus;

namespace DaisyControl_AI.Core.Core.Decisions.Goals
{
    public static class GoalsDecisionManager
    {
        private static DaisyControlStorageUsersClient usersHttpClient = new();
        private static Random random = new Random(DateTime.Now.Millisecond);

        public static async Task<bool> ReflectOnImmediateGoalsForNextAvailableUser()
        {
            DaisyControlGetUsersResponseDto usersDto = await usersHttpClient.GetUsersWithOldestImmediateGoalsRefreshTimeAsync(1);

            if (usersDto == null)
            {
                return false;
            }

            DaisyControlUserDto userToProcess = null;
            foreach (DaisyControlUserDto user in usersDto.Users)
            {
                user.NextMessageToProcessOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(30);
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
                return false;
            }

            try
            {
                // We have checked in a User to process its goals, let's do it
                DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(userToProcess).ConfigureAwait(false);
                if (userToProcess.AIImmediateGoals.Count > 0)
                {
                    // TODO: Validate if the goals are met by using the Inference server with special context
                    // TODO: substract from ImmediateGoals if the goal was met
                }

                // After validating current goals, if we don't have much goals left, we can generate some more
                if (userToProcess.AIImmediateGoals.Count <= random.Next(0, 3))
                {
                    int nbImmediateGoalsToGenerate = random.Next(0, 5);

                    // Very first immediate goals for Daisy is to find out the most BASIC information about the user (his name, gender, age, etc)
                    List<DaisyGoal> BasicUserInfoGoals = await GenerateBasicUserInfoImmediateGoals(daisyMind, nbImmediateGoalsToGenerate);

                    if (BasicUserInfoGoals.Any())
                    {
                        userToProcess.AIImmediateGoals.AddRange(BasicUserInfoGoals);
                    }

                    if (userToProcess.AIImmediateGoals.Count < nbImmediateGoalsToGenerate)
                    {
                        // TODO: generate more immediate goals
                    }
                }

                if ((DateTime.UtcNow - userToProcess.MessagesHistory.Max(m => m.CreatedAtUtc)).TotalMinutes <= 5)
                {
                    // Set the next ReflectOnImmediateGoalsForNextAvailableUser for this user to the next 5 min, as this is a live conversation with the user
                    userToProcess.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddMinutes(5);
                } else
                {
                    // Note that when we'll receive a new message and its processed, this will go down to ~5min automatically
                    userToProcess.NextImmediateGoalOperationAvailabilityAtUtc = DateTime.UtcNow.AddDays(1);
                }

                return true;

            } catch (Exception e)
            {
                return false;
            } finally
            {
                userToProcess.Status = UserStatus.Ready;
                await usersHttpClient.UpdateUserAsync(userToProcess);
            }
        }

        private static async Task<List<DaisyGoal>> GenerateBasicUserInfoImmediateGoals(DaisyControlMind daisyMind, int maxElements)
        {
            List<DaisyGoal> goals = new();

            // Name (first name + last name)
            if (string.IsNullOrWhiteSpace(daisyMind.DaisyMemory.User.Global.UserInfo.FirstName) ||
                daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.ToLowerInvariant().Trim() == "unknown" ||
                string.IsNullOrWhiteSpace(daisyMind.DaisyMemory.User.Global.UserInfo.LastName) ||
                daisyMind.DaisyMemory.User.Global.UserInfo.LastName.ToLowerInvariant().Trim() == "unknown")
            {
                var getUserNameGoal = new DaisyGoal();

                if (daisyMind.DaisyMemory.User.Global.UserInfo.FirstName != null &&
                    daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.ToLowerInvariant().Trim() != "unknown")
                {
                    getUserNameGoal.PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what {{{{user}}}}'s last name is. You only know that his first name is {daisyMind.DaisyMemory.User.Global.UserInfo.FirstName}";
                } else
                {
                    getUserNameGoal.PromptInjectionGoal = "One of {{char}} goal for the current conversation with {{user}} is to find out what {{user}}'s name is.";
                }

                goals.Add(getUserNameGoal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Age
            if (daisyMind.DaisyMemory.User.Global.UserInfo.Age == null)
            {
                var getUserAgeGoal = new DaisyGoal
                {
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out how old {{{{user}}}} is."
                };

                goals.Add(getUserAgeGoal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Genitals
            if (daisyMind.DaisyMemory.User.Global.UserInfo.Age == null)
            {
                var getUserAgeGoal = new DaisyGoal
                {
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what kind of genitals {{{{user}}}} has between {string.Join(",", Enum.GetValues<Genitals>())}. Be subtil in your approach. You could for example ask for {{{{user}}}}'s gender or sex. If {{{{user}}}}'s answer is vague,you can confirm more bluntly with {{{{user}}}}."
                };

                goals.Add(getUserAgeGoal);
            }

            return goals;
        }
    }
}
