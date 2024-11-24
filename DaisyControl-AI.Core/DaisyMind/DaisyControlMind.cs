using DaisyControl_AI.Core.DaisyMind.DaisyMemory;

namespace DaisyControl_AI.Core.DaisyMind
{
    /// <summary>
    /// Contains everything DaisyControl know about a specific User. Their relationship, who is user, what they like, dislike, what items they have, but also how to react to user, etc.
    /// This will set AI goals, purpose, but also activities, etc.
    /// </summary>
    public class DaisyControlMind
    {
        public DaisyControlMemory DaisyMemory { get; set; }
    }
}
