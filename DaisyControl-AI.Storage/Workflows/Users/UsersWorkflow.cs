
using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.RequestExecutors.Users;

namespace DaisyControl_AI.Storage.Workflows.Main
{
    public class UsersWorkflow : IUsersWorkflow
    {
        private IUsersDal daisyControlDal = null;

        public UsersWorkflow(IUsersDal daisyControlDal)
        {
            this.daisyControlDal = daisyControlDal;
        }

        public async Task<object> ExecuteAsync(IStorageDto postDto)
        {
            if (postDto == null)
            {
                return null;
            }

            var executor = UsersRequestExecutorFactory.GenerateExecutor(daisyControlDal, postDto);

            if (executor == null)
            {
                return null;// TODO: throw?
            }

            await executor.ExecuteAsync();
            return await executor.GetResponseAsync();
        }
    }
}
