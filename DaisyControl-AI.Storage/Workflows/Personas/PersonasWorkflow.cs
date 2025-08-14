
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.RequestExecutors.Personas;

namespace DaisyControl_AI.Storage.Workflows.Main
{
    public class PersonasWorkflow : IPersonasWorkflow
    {
        private IPersonasDal daisyControlDal = null;

        public PersonasWorkflow(IPersonasDal daisyControlDal)
        {
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<object> ExecuteAsync(IStorageDto postDto)
        {
            if (postDto == null)
            {
                return null;
            }

            var executor = PersonasRequestExecutorFactory.GenerateExecutor(daisyControlDal, postDto);

            if (executor == null)
            {
                return null;// TODO: throw?
            }

            await executor.ExecuteAsync();
            return await executor.GetResponseAsync();
        }
    }
}
