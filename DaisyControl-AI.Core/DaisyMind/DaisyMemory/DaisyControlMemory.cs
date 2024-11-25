using DaisyControl_AI.Core.DaisyMind.DaisyMemory.LifeEvents;
using DaisyControl_AI.Core.DaisyMind.DaisyMemory.User;

namespace DaisyControl_AI.Core.DaisyMind.DaisyMemory
{
    /// <summary>
    /// This pretty much act like the AI memory. What it know about the user, their relationship, but also the AI personality, mood, etc.
    /// This also serves to dynamically build the AI Context (llm).
    /// </summary>
    public class DaisyControlMemory
    {
        /// <summary>
        /// The AI is built to simulate what and how a real person lives. To that end, her day is a succession of "life events", such as eating a breakfast, brushing her teeth, going to work, meeting a colleague, going to the gym, talking to user, etc.
        /// The list is ordered by oldest to newest.
        /// </summary>
        public List<IAILifeEvent> EventsHistory { get; set; }
        public DaisyControlUserMemory User { get; set; }
    }
}
