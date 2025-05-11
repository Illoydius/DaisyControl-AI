using System.Collections.Generic;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Response.Users;
using DaisyControl_AI.Storage.Dtos.User;
using UserStatus = DaisyControl_AI.Storage.Dtos.UserStatus;

namespace DaisyControl_AI.Core.Core.Decisions.Goals
{
    internal static class GoalsDecisionManager
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
                return false;
            }

            try
            {
                // We have checked in a User to process its goals, let's do it
                DaisyControlMind daisyMind = await DaisyMindFactory.GenerateDaisyMind(userToProcess).ConfigureAwait(false);
                if (userToProcess.AIImmediateGoals.Count > 0)
                {
                    // Validate if the goals are met by using the Inference server with special context
                    List<DaisyGoal> beforeGoals = new();
                    beforeGoals.AddRange(userToProcess.AIImmediateGoals);

                    foreach (DaisyGoal goal in beforeGoals.Where(w => w != null))
                    {
                        if (GoalsDecisionValidator.ValidateGoal(userToProcess, goal))
                        {
                            // The goal was validated and is completed, we can remove it
                            userToProcess.AIImmediateGoals.Remove(goal);
                        } else
                        {
                            // The goal wasn't accomplished yet. We'll queue an inference task to Analyze the conversation with User to check if the goal was accomplished since the last time we analyzed the chat
                            var inferenceTask = await GenerateNewInferenceTaskFromGoal(goal);

                            if (!userToProcess.InferenceTasks.Select(s => s.KeyValue).Contains(inferenceTask.KeyValue))
                            {
                                userToProcess.InferenceTasks.Add(inferenceTask);
                            }
                        }
                    }
                }

                // After validating current goals, if we don't have much goals left, we can generate some more
                bool lowFamiliarity = userToProcess.UserInfo.GetFamiliarityPercentage() <= 15;
                int maxImmediateGoals = lowFamiliarity ? 10 : 3;
                if (userToProcess.AIImmediateGoals.Count <= maxImmediateGoals)
                {
                    int nbImmediateGoalsToGenerate = random.Next(1, 6);

                    if (lowFamiliarity)
                    {
                        nbImmediateGoalsToGenerate = 10;
                    }

                    // Very first immediate goals for Daisy is to find out the most BASIC information about the user (his name, gender, age, etc)
                    List<DaisyGoal> BasicUserInfoGoals = await GenerateBasicUserInfoImmediateGoals(daisyMind, nbImmediateGoalsToGenerate);

                    if (BasicUserInfoGoals.Any())
                    {
                        userToProcess.AIImmediateGoals.AddRange(BasicUserInfoGoals.Where(w => !userToProcess.AIImmediateGoals.Select(s => s.GoalMemoryKey).Contains(w.GoalMemoryKey)).ToArray());
                    }

                    // TODO: Get to know user with more sensitive questions (ex: kinks, fetishes, sexual experiences, etc)

                    if (userToProcess.AIImmediateGoals.Count < nbImmediateGoalsToGenerate)
                    {
                        // TODO: generate more immediate goals
                    }
                }

                LoggingManager.LogToFile("f2913016-3a0a-4625-af53-e941f660b26f", $"User linked AI has [{userToProcess.AIImmediateGoals.Count}] immediate goals.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

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

        private static async Task<InferenceTask> GenerateNewInferenceTaskFromGoal(DaisyGoal goal)
        {
            return new()
            {
                KeyType = InferenceTaskKeyType.GoalValidation,
                KeyValue = goal.GoalMemoryKey,
                Prompt = goal.PromptGoalValidationInjection
            };
        }

        // Make sure to match it with GoalsDecisionValidator.cs
        private static async Task<List<DaisyGoal>> GenerateBasicUserInfoImmediateGoals(DaisyControlMind daisyMind, int maxElements)
        {
            List<DaisyGoal> goals = new();

            // First Name
            if (string.IsNullOrWhiteSpace(daisyMind.DaisyMemory.User.Global.UserInfo.FirstName) || daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.ToLowerInvariant().Trim() == "unknown")
            {
                DaisyGoal goal = new()
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.FirstName).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what {{{{user}}}}'s first name is. \"Uknown\" is not a valid name.",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s first name. Ignore {{{{user}}}} last name. \"Uknown\" is not a valid name.",
                };

                goals.Add(goal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Last Name
            if (string.IsNullOrWhiteSpace(daisyMind.DaisyMemory.User.Global.UserInfo.LastName) || daisyMind.DaisyMemory.User.Global.UserInfo.LastName.ToLowerInvariant().Trim() == "unknown")
            {
                DaisyGoal goal = new()
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.LastName).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what {{{{user}}}}'s last name is. Be subtle about it as most people don't want to give their last name easily. You may use your cunning nature to get that information. You could for instance give your own last name to {{{{user}}}}, to incite them to give your theirs. \"Uknown\" is not a valid name.",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s last name. Ignore {{{{user}}}} first name. \"Uknown\" is not a valid name.",
                };

                goals.Add(goal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Age
            if (daisyMind.DaisyMemory.User.Global.UserInfo.Age == null)
            {
                var goal = new DaisyGoal
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.Age).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out how old {{{{user}}}} is.",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s age.",
                };

                goals.Add(goal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Gender
            if (daisyMind.DaisyMemory.User.Global.UserInfo.Gender == null)
            {
                var goal = new DaisyGoal
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.Gender).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what gender {{{{user}}}} is between {string.Join(",", Enum.GetValues<Gender>())}. Be subtil in your approach.",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s gender.",
                };

                goals.Add(goal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Genitals
            if (daisyMind.DaisyMemory.User.Global.UserInfo.Age == null)
            {
                var goal = new DaisyGoal
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.Genitals).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what kind of genitals {{{{user}}}} has between {string.Join(",", Enum.GetValues<Genitals>())}. Be subtil in your approach. You could for example ask for {{{{user}}}}'s gender or sex. If {{{{user}}}}'s answer is vague,you can confirm more bluntly with {{{{user}}}}.",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s genitals. If {{{{user}}}} gave you his gender or sex, you can guess {{{{user}}}} genitals from that information. {{{{user}}}} genitals can either be {Genitals.Penis} or {Genitals.Vagina}.",
                };

                goals.Add(goal);
            }

            if (goals.Count >= maxElements)
            {
                return goals;
            }

            // Work
            if (string.IsNullOrWhiteSpace(daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupation.WorkTitle))
            {
                var goal = new DaisyGoal
                {
                    ValidationType = GoalValidationType.UserInfo,
                    GoalMemoryKey = nameof(daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupation.WorkTitle).ToLowerInvariant(),
                    PromptInjectionGoal = $"One of {{{{char}}}} goal for the current conversation with {{{{user}}}} is to find out what {{{{user}}}} does for work. Is he an employee? Where does he work? What does he do? What is the title of his job (accountant,cashier,software engineer,etc)?",
                    PromptGoalValidationInjection = $"Analyze the conversation between {{{{char}}}} and {{{{user}}}} to find {{{{user}}}}'s work title (ex: accountant,cashier,software engineer,...).",
                };

                goals.Add(goal);
            }

            return goals;
        }
    }
}
