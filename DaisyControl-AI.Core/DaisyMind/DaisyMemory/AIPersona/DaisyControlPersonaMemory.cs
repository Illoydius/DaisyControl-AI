using DaisyControl_AI.Core.DaisyMind.DaisyMemory.Schedule;
using DaisyControl_AI.Storage.Dtos.AIPersona;

namespace DaisyControl_AI.Core.DaisyMind.DaisyMemory.User
{
    /// <summary>
    /// Memory related to an AI Persona, specifically.
    /// </summary>
    public class DaisyControlPersonaMemory
    {
        public DaisyControlAIPersonaDto Global { get; set; }
        public DaisyControlScheduleMemory Schedule { get; set; }
    }
}
