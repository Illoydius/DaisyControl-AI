using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Core.DaisyMind
{
    public static class DaisyMindFactory
    {
        public static async Task<DaisyControlMind> GenerateDaisyMind(DaisyControlUserDto userDto)
        {
            if (userDto == null)
            {
                LoggingManager.LogToFile("436c7c12-99b3-4551-8811-aa1dff56c681", $"Couldn't generate DaisyMind due to null userDto. Failed to Get or Add/Get User to storage.");
            }

            return new DaisyControlMind()
            {
                DaisyMemory = new()
                {
                    //EventsHistory = Generate from DB..
                    User = new()
                    {
                        Global = userDto,
                    },
                }
            };
        }
    }
}
