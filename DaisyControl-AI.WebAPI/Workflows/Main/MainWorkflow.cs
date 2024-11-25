
using DaisyControl_AI.WebAPI.Dtos;

namespace DaisyControl_AI.WebAPI.Workflows.Main
{
    public class MainWorkflow : IWorkflow
    {
        public MainWorkflow()// TODO: Add DI
        {
            
        }

        public Task<string> Post(IDto postDto)
        {
            //if (postDto is not MainPostSendMessageToAIDto mainPostSendMessageToAIDto)
            //{
            //    // TODO: return exception
            //    return null;
            //}

            //// Generate persona prompt from current AI Context
            //var prompt = promptGenerator.GenerateCurrentAICustomizationPrompt();

            //// Add text history for the remaning tokens space minus the new user's message
            //var personaPromptNbTokens = MachineLearningTokensUtils.CalculateNbTokensInString(string.Join(string.Empty, prompt.Messages.Select(sm => sm.Content)));
            //var newUserMessageNbTokens = MachineLearningTokensUtils.CalculateNbTokensInString(mainPostSendMessageToAIDto.Message);
            //var nbTokensInHistoryToAccept = promptGenerator.NbMaxTokensInPrompt - personaPromptNbTokens - newUserMessageNbTokens;

            //var historyMessages = textHistoryManager.GetMostRecentMessages(nbTokensInHistoryToAccept);

            //foreach (var historyMessage in historyMessages)
            //{
            //    prompt.Messages.Add(historyMessage);
            //}

            //// Add user's message
            //prompt.Messages.Add(new InferencePromptMessageModel
            //{
            //    Role = InferencePromptRole.user.ToString(),
            //    Content = mainPostSendMessageToAIDto.Message,
            //});

            //var AIResponse = await nitroManager.GenerateAnswerFromAI(prompt);

            //// Register user message and response to textHistory
            //textHistoryManager.RegisterNewMessage(InferencePromptRole.user, mainPostSendMessageToAIDto.Message);
            //textHistoryManager.RegisterNewMessage(InferencePromptRole.assistant, AIResponse);

            //return AIResponse;
            return null;
        }
    }
}
