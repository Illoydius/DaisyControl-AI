using DaisyControl_AI.Common.Exceptions.HTTP;
using DaisyControl_AI.Storage.Dtos.Requests;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IWorkflow workflow;

        public UsersController(IWorkflow workflow)
        {
            this.workflow = workflow;
        }

        // TEMP
        /// <summary>
        /// Just a ping-like endpoint.
        /// </summary>
        [HttpGet]
        [Route("teapot")]
        public async Task<ActionResult<object>> Teapot(string userId)
        {
            return "I am a teapot";
        }

        /// <summary>
        /// Get new User from storage.
        /// </summary>
        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult<object>> GetUser(DaisyControlGetUserRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);

            //if (response == null)
            //{
            //    return NotFound();
            //}

            return response;
        }

        /// <summary>
        /// Add new User to storage.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> AddUser([FromBody] DaisyControlAddUserRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Update (full obj) User to storage.
        /// </summary>
        [HttpPut]
        [Route("{userIdToUpdate}")]
        public async Task<ActionResult<object>> UpdateUser([FromRoute] string UserIdToUpdate, DaisyControlUpdateUserRequestDto userRequest)
        {
            // Validate request
            if (userRequest.Id != UserIdToUpdate)
            {
                throw new BadRequestWebApiException("d117c895-568d-4376-a785-466fcc8b6f7b", $"UserId [{UserIdToUpdate}] to update didn't match the provided body UserId [{userRequest.Id}].");
            }

            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Delete User from storage.
        /// </summary>
        [HttpDelete]
        [Route("{userId}")]
        public async Task<ActionResult<object>> DeleteUser(DaisyControlDeleteUserRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Get a chunk of users with unprocessed messages.
        /// </summary>
        [HttpGet]
        [Route("unprocessedUsersMessages")]
        public async Task<ActionResult<object>> UsersWithUnprocessedUserMessages(DaisyControlGetUsersWithUnprocessedUserMessagesRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Get a chunk of users with unprocessed messages.
        /// </summary>
        [HttpGet]
        [Route("unprocessedAIMessages")]
        public async Task<ActionResult<object>> UsersWithUnprocessedAIMessages(DaisyControlGetUsersWithUnprocessedAIMessagesRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }

        /// <summary>
        /// Get a chunk of users with 'working' status.
        /// </summary>
        [HttpGet]
        [Route("working")]
        public async Task<ActionResult<object>> UsersWithWorkingStatus(DaisyControlGetUsersWithWorkingStatusRequestDto userRequest)
        {
            object response = await workflow.ExecuteAsync(userRequest);
            return response;
        }
    }
}
