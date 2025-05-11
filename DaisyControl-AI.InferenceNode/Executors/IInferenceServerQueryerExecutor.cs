using DaisyControl_AI.Storage.Dtos;

namespace DaisyControl_AI.InferenceNode.Executors
{
    internal interface IInferenceServerQueryerExecutor
    {
        Task<string> Execute();
        Task<bool> SaveResult(string queryJsonResult);
    }
}
