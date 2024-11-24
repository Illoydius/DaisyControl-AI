using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public static class MainRequestExecutorFactory
    {
        public static IMainRequestExecutor GenerateExecutor(IDaisyControlDal daisyControlDal, IDto postDto)
        {
            switch (postDto)
            {
                case DaisyControlAddUserDto daisyControlAddUserDto:
                    return new DaisyControlAddUserRequestExecutor(daisyControlDal, daisyControlAddUserDto);
                default:
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
