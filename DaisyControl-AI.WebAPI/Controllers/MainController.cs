using System.Text.Json;
using DaisyControl_AI.WebAPI.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.WebAPI.Controllers
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
