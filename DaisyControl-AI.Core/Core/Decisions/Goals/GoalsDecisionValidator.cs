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
            // Also handle the familiarity in DaisyControlUserInfo.cs
            switch (goal.GoalMemoryKey.ToLowerInvariant())
            {
                case "firstname":
                    return !(string.IsNullOrWhiteSpace(userToProcess.UserInfo.FirstName) || userToProcess.UserInfo.FirstName.ToLowerInvariant().Trim() == "unknown");
                case "lastname":
                    return !(string.IsNullOrWhiteSpace(userToProcess.UserInfo.LastName) || userToProcess.UserInfo.LastName.ToLowerInvariant().Trim() == "unknown");
                case "email":
                    return !(string.IsNullOrWhiteSpace(userToProcess.UserInfo.Email));
                case "age":
                    return userToProcess.UserInfo.Age != null;
                case "gender":
                    return userToProcess.UserInfo.Gender != null;
                case "genitals":
                    return userToProcess.UserInfo.Genitals != null;
                case "countryName":
                    return !string.IsNullOrWhiteSpace(userToProcess.UserInfo.Location.CountryName);
                case "worktitle":
                    return userToProcess.UserInfo.WorkOccupation.WorkTitle != null;
                case "annualsalary":
                    return userToProcess.UserInfo.WorkOccupation.AnnualSalary != null;
                case "companyname":
                    return !string.IsNullOrWhiteSpace(userToProcess.UserInfo.WorkOccupation.Company.Name);
                case "companyaddress":
                    return !string.IsNullOrWhiteSpace(userToProcess.UserInfo.WorkOccupation.Company.Address);
                case "workdescriptionsummary":
                    return !string.IsNullOrWhiteSpace(userToProcess.UserInfo.WorkOccupation.WorkDescriptionSummary);
                default:
                    LoggingManager.LogToFile("3dc6a7f7-a5cd-475e-9396-6b434abb108c", $"Unhandled GoalMemoryKey [{goal.GoalMemoryKey}].");
                    break;
            }

            return false;
        }
    }
}
