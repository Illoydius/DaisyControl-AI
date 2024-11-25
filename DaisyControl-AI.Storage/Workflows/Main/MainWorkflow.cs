
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.Workflows.Main
{
    public class MainWorkflow : IWorkflow
    {
        private IDaisyControlDal daisyControlDal = null;

        public MainWorkflow(IDaisyControlDal daisyControlDal)
        {
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<object> ExecuteAsync(IStorageDto postDto)
        {
            if (postDto == null)
            {
                return null;
            }

            var executor = MainRequestExecutorFactory.GenerateExecutor(daisyControlDal, postDto);

            if (executor == null)
            {
                return null;// TODO: throw?
            }

            await executor.ExecuteAsync();
            return await executor.GetResponseAsync();
        }
    }
}
