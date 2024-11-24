using DaisyControl_AI.Storage.Dtos;

namespace DaisyControl_AI.Storage.Workflows
{
    public interface IWorkflow
    {
        Task<string> ExecuteAsync(IDto dto);
    }
}
