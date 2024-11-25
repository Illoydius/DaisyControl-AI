using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests;

namespace DaisyControl_AI.Storage.RequestExecutors.Main
{
    public static class MainRequestExecutorFactory
    {
        public static IMainRequestExecutor GenerateExecutor(IDaisyControlDal daisyControlDal, IStorageDto postDto)
        {
            switch (postDto)
            {
                case DaisyControlAddUserRequestDto daisyControlAddUserDto:
                    return new DaisyControlAddUserRequestExecutor(daisyControlDal, daisyControlAddUserDto);
                    case DaisyControlUpdateUserRequestDto daisyControlUpdateUserDto:
                    return new DaisyControlUpdateUserRequestExecutor(daisyControlDal, daisyControlUpdateUserDto);
                case DaisyControlGetUserRequestDto daisyControlGetUserDto:
                    return new DaisyControlGetUserRequestExecutor(daisyControlDal, daisyControlGetUserDto);
                default:
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
