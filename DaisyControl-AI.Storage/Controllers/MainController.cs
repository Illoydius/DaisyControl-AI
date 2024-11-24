using System.Text.Json;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        private IWorkflow workflow;

        public MainController(IWorkflow workflow)
        {
            this.workflow = workflow;
        }

        /// <summary>
        /// Add new User to storage.
        /// </summary>
        [Route("users")]
        [HttpPost]
        public async Task<ActionResult<string>> AddUser([FromBody] DaisyControlAddUserDto user)
        {
            string response = await workflow.ExecuteAsync(user);
            return JsonSerializer.Serialize(response);
        }

        /// <summary>
        /// Just a ping-like endpoint.
        /// </summary>
        [Route("users/{userId}/teapot")]
        [HttpPost]
        public async Task<ActionResult<string>> Teapot(string userId)
        {
            return JsonSerializer.Serialize("I am a teapot");
        }
    }
}
