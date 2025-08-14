using System.Text;
using DaisyControl_AI.Core.DaisyMind;
using DaisyControl_AI.Storage.Dtos;
using DaisyControl_AI.Storage.Dtos.User;
using Humanizer;

namespace DaisyControl_AI.Core.InferenceServer.Context
{
    public static class AskForFollowUpContextBuilder
    {
        public static string BuildContext(DaisyControlMind daisyMind, DaisyControlUserDto user)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"<|im_start|>system");

            // Start with the basic context around what is expected from the AI for goal validation
            BuildBasicAIContext(stringBuilder, daisyMind);

            // AI description, appearance, personality, mood, etc.
            BuildAICharacterSheet(stringBuilder, daisyMind);
            BuildUserCharacterSheet(stringBuilder, daisyMind);

            stringBuilder.AppendLine($"Analyze the conversation between {{{{char}}}} and {{{{user}}}}. Determine if it would make sense that {{{{char}}}} send another message without waiting for {{{{user}}}} to reply.");

            if (daisyMind.DaisyMemory.User.Global.MessagesHistory.Count(w => w.ReferentialType == MessageReferentialType.User) > 0 && daisyMind.DaisyMemory.User.Global.MessagesHistory.Count(w => w.ReferentialType == MessageReferentialType.Assistant) > 0)
            {
                // Handle 'a long time since last message from user'
                DateTime? lastMessageDate = daisyMind.DaisyMemory.User.Global.MessagesHistory.Where(w => w.ReferentialType == MessageReferentialType.User).OrderByDescending(l => l.CreatedAtUtc.DateTime).FirstOrDefault()?.CreatedAtUtc.DateTime;
                if (lastMessageDate.HasValue && (DateTime.UtcNow - lastMessageDate.Value).TotalSeconds >= 60)
                {
                    stringBuilder.AppendLine($"Please note that {{{{user}}}} last message was {lastMessageDate.Humanize()}.");
                }

                DateTime? lastMessageDateDaisy = daisyMind.DaisyMemory.User.Global.MessagesHistory.Where(w => w.ReferentialType == MessageReferentialType.Assistant).OrderByDescending(l => l.CreatedAtUtc.DateTime).FirstOrDefault()?.CreatedAtUtc.DateTime;
                if (lastMessageDateDaisy.HasValue && (DateTime.UtcNow - lastMessageDateDaisy.Value).TotalSeconds >= 60)
                {
                    stringBuilder.AppendLine($"Please note that {{{{char}}}} last message was {lastMessageDateDaisy.Humanize()} and {{{{char}}}} may become impatient, depending on {{{{char}}}} personality and current mood. Would it make sense for {{{{char}}}} to desire to follow up with another message?");
                }
            }

            stringBuilder.AppendLine($"Please consider that some personality traits such as impatience, arrogance, etc could influence the desire to send another message immediately without waiting for a response.");

            BuildOutputFormat(stringBuilder, daisyMind);
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
            context = context.Replace("{{char}}", daisyMind.DaisyMemory.Self.Global.PersonaInfo.FirstName);

            File.WriteAllText("AskForFollowUpContextBuilder-last.txt", context);

            return context;
        }

        private static void BuildOutputFormat(StringBuilder stringBuilder, DaisyControlMind daisyMind)
        {
            stringBuilder.AppendLine($"Answer either with 'yes', 'no', 'true' or 'false' please.");
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
            stringBuilder.AppendLine($"Occupation({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupationCategory.WorkTitle ?? "unknown work title"})");
            stringBuilder.AppendLine($"Workplace({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupationCategory.Company.Name ?? "unknown company name"}({daisyMind.DaisyMemory.User.Global.UserInfo.WorkOccupationCategory.Company.Name ?? "unknown workplace address"}))]");
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
                        referentialName = daisyMind.DaisyMemory.Self.Global.PersonaInfo.FirstName;
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


