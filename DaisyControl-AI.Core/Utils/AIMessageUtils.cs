
using DaisyControl_AI.Common.Diagnostics;
using DaisyControl_AI.Core.DaisyMind;

namespace DaisyControl_AI.Core.Utils
{
    public static class AIMessageUtils
    {
        public static string CleanAIResponse(DaisyControlMind daisyMind, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return string.Empty;
                }

                if (daisyMind == null)
                {
                    return text;
                }

                // Clean the response to remove certain tags
                string responseText = text.Trim();

                if (responseText.ToLowerInvariant().StartsWith("assistant"))
                {
                    responseText = responseText.Substring(9, responseText.Length - 9).TrimStart();
                }

                if (responseText.ToLowerInvariant().StartsWith($"{daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.ToLowerInvariant()}:"))
                {
                    responseText = responseText.Substring(daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.Length + 1, responseText.Length - daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.Length - 1).TrimStart();
                }

                if (responseText.ToLowerInvariant().StartsWith($"{daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.ToLowerInvariant()} :"))
                {
                    responseText = responseText.Substring(daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.Length + 2, responseText.Length - daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName.Length - 2).TrimStart();
                }

                return responseText;
            } catch (Exception e)
            {
                LoggingManager.LogToFile("f870c965-ed8c-498b-9ae7-b9d5622eced3", $"Couldn't clean AI response [{text}]. Original message will be used.", e);
                return text;
            }
        }
    }
}
