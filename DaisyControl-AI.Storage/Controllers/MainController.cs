using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        private IWorkflow workflow;

        public MainController(IWorkflow workflow)
        {
            this.workflow = workflow;
        }

        /// <summary>
        /// Get new User from storage.
        /// </summary>
        [HttpGet]
        [Route("users/{userId}")]
        public async Task<ActionResult<object>> GetUser(DaisyControlGetUserRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);

            if (response == null)
            {
                return NotFound();
            }

            return response;
        }

        /// <summary>
        /// Add new User to storage.
        /// </summary>
        [Route("users")]
        [HttpPost]
        public async Task<ActionResult<object>> AddUser([FromBody] DaisyControlAddUserRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Just a ping-like endpoint.
        /// </summary>
        [Route("users/{userId}/teapot")]
        [HttpPost]
        public async Task<ActionResult<object>> Teapot(string userId)
        {
            return "I am a teapot";
        }
    }
}
