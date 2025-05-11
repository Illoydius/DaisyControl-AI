using System.Text.RegularExpressions;

namespace DaisyControl_AI.Common.Utils
{
    public class MessagingUtils
    {
        public static string RemoveAIThougths(string messageContent)
        {
            return Regex.Replace(messageContent, @"\*\*.*?\*\*", "");
        }
    }
}
