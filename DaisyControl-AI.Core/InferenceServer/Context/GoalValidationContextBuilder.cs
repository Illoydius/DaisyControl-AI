using System.Text;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Core.InferenceServer.Context
{
    public static class GoalValidationContextBuilder
    {
        public static string BuildContext(DaisyControlMind daisyMind, DaisyControlUserDto user, string taskKey, string taskPrompt)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"<|im_start|>system");

            // Start with the basic context around what is expected from the AI for goal validation
            BuildBasicAIContext(stringBuilder, daisyMind);

            // AI description, appearance, personality, mood, etc.
            BuildAICharacterSheet(stringBuilder, daisyMind);
            BuildUserCharacterSheet(stringBuilder, daisyMind);
            BuildOutputFormat(stringBuilder, daisyMind, taskKey);

            stringBuilder.AppendLine(taskPrompt);

            BuildSummaryOfOldChat(stringBuilder, daisyMind);

            stringBuilder.Append($"<|im_end|>");
            stringBuilder.AppendLine(string.Empty);

            BuildMessagesHistoryUsingRemainingSpace(stringBuilder, daisyMind);

            string context = stringBuilder.ToString();
            string interlocutorRef = daisyMind.DaisyMemory.User.Global.UserInfo.FirstName;
            if (interlocutorRef.ToLowerInvariant().Trim() == "unknown")
            {
                interlocutorRef = $"the person {{{{char}}}} is communicating with";
            }
            context = context.Replace("{{user}}", interlocutorRef);
            context = context.Replace("{{char}}", daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName);

            return context;
        }

        private static void BuildOutputFormat(StringBuilder stringBuilder, DaisyControlMind daisyMind, string key)
        {
            stringBuilder.AppendLine($"You must format your reply in the following Json format: {{\"{key}\":\"[VALUE FOUND FROM THE CONVERSATION]\"}}. Your reply must ONLY contains the JSON.");
            stringBuilder.AppendLine($"If you can't find {key} from the conversation, do not invent an answer for [VALUE FOUND FROM THE CONVERSATION], leave it empty or null.");
        }

        private static void BuildBasicAIContext(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            stringBuilder.AppendLine($"You are an helpful assistant.");
            stringBuilder.AppendLine($"{{{{char}}}} is having a conversation with {{{{user}}}} via text messages on Discord, a chat application on {{{{char}}}} cellphone.");
        }

        private static void BuildSummaryOfOldChat(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            // TODO: add summary for older chat logs
        }

        private static void BuildAICharacterSheet(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            stringBuilder.AppendLine($"Here's the description of {{{{char}}}}:");
            ContextBuilder.BuildAICharacterSheet(stringBuilder, daisyMind, false);
        }

        private static void BuildUserCharacterSheet(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            stringBuilder.AppendLine($"Here's the description of {{{{user}}}}:");
            stringBuilder.AppendLine($"[Name({daisyMind.DaisyMemory.User.Global.UserInfo.FirstName ?? "unknown first name"} {daisyMind.DaisyMemory.User.Global.UserInfo.LastName ?? "unknown last name"})");
            stringBuilder.AppendLine($"Gender({daisyMind.DaisyMemory.User.Global.UserInfo.Gender?.ToString() ?? "unknown"})");
            stringBuilder.AppendLine($"Genitals({daisyMind.DaisyMemory.User.Global.UserInfo.Genitals?.ToString() ?? "unknown"})");
            stringBuilder.AppendLine($"Age({daisyMind.DaisyMemory.User.Global.UserInfo.Age?.ToString() ?? "unknown"})");
            //stringBuilder.AppendLine($"Body(Slim Physique + long hair + brown hair + straight hair tied in a bun with bangs + brown eyes + Narrow Hips + Medium Breasts + Small Butt + Slim Waist + Slender Thighs + Smooth Skin)");
            //stringBuilder.AppendLine($"Personality(Assertive + Confident + Decisive + Clingy + Natural Leader + Strategic Thinker + Charismatic + Perceptive + Ambitious + Self-Reliant + Disciplined + Empathetic + Dominant)");
            stringBuilder.AppendLine($"Occupation({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupation.WorkTitle ?? "unknown work title"})");
            stringBuilder.AppendLine($"Workplace({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupation.Company.Name ?? "unknown company name"}({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupation.Company.Name ?? "unknown workplace address"}))]");
            //stringBuilder.AppendLine($"Likes(Dogs + Sunsets + Books + Feeling wanted + Control + Romantic Movies + Tv Shows + Fiction + Obedience + Submissive men)");
            //stringBuilder.AppendLine($"Dislikes(Abandonment + Being alone + Disrespect)");
            //stringBuilder.AppendLine($"Speech(She has a soft, but strict and charismatic voice.)");
            //stringBuilder.AppendLine($"Background({{{{char}}}} is living by her own in San Francisco, in a luxurious condo with her dog 'Buttercup'. She was raised in California in a happy family home before moving out at the age of 23. She studied to be an upper level manager. She is a very successful woman, respected by her peers for her work dedication)");
            //stringBuilder.AppendLine($"Clothing(woman suit + elegant pants + red high heels + White lace bra + White lace panties.)");
        }

        private static void BuildMessagesHistoryUsingRemainingSpace(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            if (daisyMind.DaisyMemory.User.Global.MessagesHistory == null)
            {
                return;
            }

            stringBuilder.AppendLine($"<|im_start|>system\r\nConversation between {{{{char}}}} and {{{{user}}}}:\r\n");

            List<DaisyControlMessage> messagesToAddToContext = new();
            List<DaisyControlMessage> messagesToEvaluate = new(daisyMind.DaisyMemory.User.Global.MessagesHistory);
            messagesToEvaluate.Reverse();

            foreach (DaisyControlMessage message in messagesToEvaluate)
            {
                // TODO: check if new context size would be > max_token
                messagesToAddToContext.Add(message);
            }

            // Reverse it back to the original order
            messagesToAddToContext.Reverse();

            foreach (DaisyControlMessage message in messagesToAddToContext)
            {
                string referentialName = $"{MessageReferentialType.System}";

                switch (message.ReferentialType)
                {
                    case MessageReferentialType.User:
                        referentialName = daisyMind.DaisyMemory.User.Global.UserInfo.FirstName;
                        break;
                    case MessageReferentialType.Assistant:
                        referentialName = daisyMind.DaisyMemory.User.Global.AIGlobal.FirstName;
                        break;
                    case MessageReferentialType.System:
                        referentialName = $"system";
                        break;
                        // Ignore default
                }

                if (string.IsNullOrWhiteSpace(referentialName))
                {
                    referentialName = "Someone unknown to you";
                }

                stringBuilder.AppendLine($"{referentialName}: {message.MessageContent}");
            }

            stringBuilder.Append("<|im_end|>");
        }
    }
}


