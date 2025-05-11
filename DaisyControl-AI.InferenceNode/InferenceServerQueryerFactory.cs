using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.InferenceNode.Executors;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.InferenceNode
{
    internal static class InferenceServerQueryerFactory
    {
        public static IInferenceServerQueryerExecutor GenerateExecutor(InferenceTask inferenceTask, DaisyControlUserDto daisyControlUserDto)
        {
            switch (inferenceTask.KeyType)
            {
                case InferenceTaskKeyType.GoalValidation:
                    return new DaisyControlGoalValidationRequestExecutor(inferenceTask, daisyControlUserDto);
                    
                default:
                    LoggingManager.LogToFile("f89c03bb-50b3-4d70-b814-e409c26dda10", $"InfernceTaskKeyType is unhandled [{inferenceTask?.KeyType}].");
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
