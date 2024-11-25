using DaisyControl_AI.WebAPI.Dtos;

namespace DaisyControl_AI.WebAPI.Workflows
{
    public interface IWorkflow
    {
        Task<string> Post(IDto postDto);
    }
}
