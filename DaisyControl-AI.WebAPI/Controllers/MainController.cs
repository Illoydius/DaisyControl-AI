using DaisyControl_AI.WebAPI.Dtos;
using DaisyControl_AI.WebAPI.Workflows;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DaisyControl_AI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MainController : ControllerBase
    {
        private IWorkflow workflow;

        public MainController(IWorkflow workflow)
        {
            this.workflow = workflow;
        }

        /// <summary>
        /// Sends a new message to the AI from a user.
        /// </summary>
        /// <param name="mainPostSendMessageToAIDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> SendMessageToAI([FromBody] MainPostSendMessageToAIDto mainPostSendMessageToAIDto)
        {
            var result = await workflow.Post(mainPostSendMessageToAIDto);
            return JsonSerializer.Serialize(result);
        }
    }
}
