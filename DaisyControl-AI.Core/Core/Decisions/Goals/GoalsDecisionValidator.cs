using System.Reflection;
using System.Text.Json.Serialization;
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

        private static bool ValidateGoalAgainstUserInfoProperties(IUserInfoData dataType, string serializedPropertyName)
        {
            if (dataType == null)
            {
                return false;
            }

            var properties = dataType.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType?.GetInterfaces()?.Any(a => a == typeof(IUserInfoData)) == true)
                {
                    var data = (IUserInfoData)property.GetValue(dataType);
                    if (ValidateGoalAgainstUserInfoProperties(data, serializedPropertyName))
                    {
                        return true;
                    }

                    continue;
                }

                // Check if the property is the one we're currently validating
                if (property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name?.ToLowerInvariant() != serializedPropertyName)
                {
                    continue;
                }

                if (ValidateGoalAgainstUserInfoProperty(property, dataType, serializedPropertyName))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ValidateGoalAgainstUserInfoProperty(PropertyInfo propertyInfo, IUserInfoData userInfoData, string serializedPropertyName)
        {
            if (propertyInfo == null)
            {
                return false;
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                var strValue = propertyInfo.GetValue(userInfoData) as string;
                if (!string.IsNullOrWhiteSpace(strValue) && DaisyControlUserInfo.InvalidPropertiesValues().All(a => a != strValue.ToLowerInvariant()))
                {
                    return true;
                }
            } else
            {
                if (propertyInfo.GetValue(userInfoData) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ValidateGoalFromUserInfo(DaisyControlUserDto userToProcess, DaisyGoal goal)
        {
            return ValidateGoalAgainstUserInfoProperties(userToProcess.UserInfo, goal.GoalMemoryKey);
        }
    }
}
