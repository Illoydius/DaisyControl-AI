using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Core.Core.Decisions.Goals
{
    internal static class GoalsDecisionValidator
    {
        internal static bool ValidateGoal(DaisyControlUserDto userToProcess, DaisyGoal goal)
        {
            if (goal == null)
            {
                return false;
            }

            switch (goal.ValidationType)
            {
                case GoalValidationType.UserInfo:
                    return ValidateGoalFromUserInfo(userToProcess, goal);
                case GoalValidationType.InferenceServerQuery:
                    // TODO
                    break;
                default:
                    LoggingManager.LogToFile("7016ae82-e06e-4fd1-8cb4-6cce079dda3f", $"Unhandled GoalValidationType [{goal.ValidationType}].");
                    break;
            }

            return false;
        }

        private static bool ValidateGoalFromUserInfo(DaisyControlUserDto userToProcess, DaisyGoal goal)
        {
            // Make sure to match it with GoalsDecisionManager.cs
            switch (goal.GoalMemoryKey)
            {
                case "firstname":
                    return !(string.IsNullOrWhiteSpace(userToProcess.UserInfo.FirstName) || userToProcess.UserInfo.FirstName.ToLowerInvariant().Trim() == "unknown");
                case "lastname":
                    return !(string.IsNullOrWhiteSpace(userToProcess.UserInfo.LastName) || userToProcess.UserInfo.LastName.ToLowerInvariant().Trim() == "unknown");
                case "age":
                    return userToProcess.UserInfo.Age != null;
                case "gender":
                    return userToProcess.UserInfo.Gender != null;
                case "genitals":
                    return userToProcess.UserInfo.Genitals != null;
                case "worktitle":
                    return userToProcess.UserInfo.WorkOccupation.WorkTitle != null;
                default:
                    LoggingManager.LogToFile("219383b6-7520-4f5a-9bed-cf554742c9c2", $"Unhandled GoalMemoryKey [{goal.GoalMemoryKey}].");
                    break;
            }

            return false;
        }
    }
}
