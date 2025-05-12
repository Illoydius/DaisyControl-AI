
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

                string message = text.Replace(Environment.NewLine, string.Empty);
                string tag = "<|im_start|>assistant";
                int startPosition = message.ToLowerInvariant().IndexOf(tag);

                if (startPosition >= 0)
                {
                    message = message.Substring(startPosition + tag.Length, message.Length - startPosition - tag.Length);

                    string endTag = "<|im_end|>";
                    int endPosition = message.ToLowerInvariant().IndexOf(endTag);

                    if (endPosition >= 0)
                    {
                        message = message.Substring(0, endPosition);
                    }
                }

                // Clean the response to remove certain tags
                string responseText = message.Trim().Trim('.').Trim();

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

                if (responseText.ToLowerInvariant().StartsWith($"{daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.ToLowerInvariant()}:"))
                {
                    responseText = responseText.Substring(daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.Length + 1, responseText.Length - daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.Length - 1).TrimStart();
                }

                if (responseText.ToLowerInvariant().StartsWith($"{daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.ToLowerInvariant()} :"))
                {
                    responseText = responseText.Substring(daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.Length + 2, responseText.Length - daisyMind.DaisyMemory.User.Global.UserInfo.FirstName.Length - 2).TrimStart();
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
