using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.Requests.Personas;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.RequestExecutors.Main;

namespace DaisyControl_AI.Storage.RequestExecutors.Personas
{
    public static class PersonasRequestExecutorFactory
    {
        public static IMainRequestExecutor GenerateExecutor(IPersonasDal daisyControlDal, IStorageDto postDto)
        {
            switch (postDto)
            {
                // Personas
                case DaisyControlAddPersonaRequestDto daisyControlAddPersonaDto:
                    return new DaisyControlAddPersonaRequestExecutor(daisyControlDal, daisyControlAddPersonaDto);
                case DaisyControlUpdatePersonaRequestDto daisyControlUpdatePersonaDto:
                    return new DaisyControlUpdatePersonaRequestExecutor(daisyControlDal, daisyControlUpdatePersonaDto);
                case DaisyControlGetPersonaRequestDto daisyControlGetPersonaDto:
                    return new DaisyControlGetPersonaRequestExecutor(daisyControlDal, daisyControlGetPersonaDto);
                case DaisyControlDeletePersonaRequestDto daisyControlDeletePersonaDto:
                    return new DaisyControlDeletePersonaRequestExecutor(daisyControlDal, daisyControlDeletePersonaDto);

                default:
                    return null;// TODO : replace with unhandledExc
            }
        }
    }
}
