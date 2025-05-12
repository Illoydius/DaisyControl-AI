using System.Text.Json;
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Common.Utils
{
    public class MessagingUtils
    {
        public static string GetMessageContent(string rawMessageFromStorage)
        {
            try
            {
                return JsonSerializer.Deserialize<DaisyMessage>(rawMessageFromStorage)?.Message ?? rawMessageFromStorage;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("a0d60d52-c812-40a8-a0d1-5d530b2258e7", $"Couldn't deserialize message [{rawMessageFromStorage}] into a valid format.");
                return rawMessageFromStorage;
            }

            //return Regex.Replace(messageContent, @"\*\*.*?\*\*", "");
        }

        public static string ToDaisyMessage(string messageContent, string thoughtsContent)
        {
            return JsonSerializer.Serialize(new DaisyMessage
            {
                Message = messageContent,
                Thoughts = thoughtsContent,
            });
        }
    }
}
