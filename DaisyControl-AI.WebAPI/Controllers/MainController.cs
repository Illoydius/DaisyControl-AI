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
        [HttpGet]
        [Route("teapot")]
        public async Task<ActionResult<string>> Teapot(string userId)
        {
            return "I am a teapot";
        }
    }
}
