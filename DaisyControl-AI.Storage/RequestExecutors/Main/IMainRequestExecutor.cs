namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public interface IMainRequestExecutor
    {
        Task<bool> ExecuteAsync();
        Task<object> GetResponseAsync();
    }
}
