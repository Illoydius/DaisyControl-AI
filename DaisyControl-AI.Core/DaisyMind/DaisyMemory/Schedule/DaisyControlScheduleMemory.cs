using DaisyControl_AI.Core.DaisyMind.DaisyMemory.Schedule.Items;

namespace DaisyControl_AI.Core.DaisyMind.DaisyMemory.Schedule
{
    /// <summary>
    /// Daisy memory about her schedule. Events to come, work, take a bath, cook, gym, etc.
    /// </summary>
    public class DaisyControlScheduleMemory
    {
        public List<IDaisyControlScheduleItemMemory> ScheduleItems { get; set; } = new();
    }
}
