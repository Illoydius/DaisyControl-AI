using DaisyControl_AI.Storage.Dtos.Requests.Messages;
using DaisyControl_AI.Storage.Dtos.Requests.MessagesBuffer;
using DaisyControl_AI.Storage.Dtos.Requests.Users;
using DaisyControl_AI.Storage.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace DaisyControl_AI.Storage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesBufferController : Controller
    {
        private IWorkflow workflow;

        public MessagesBufferController(IWorkflow workflow)
        {
            this.workflow = workflow;
        }

        /// <summary>
        /// Get a pending request from storage.
        /// </summary>
        [HttpGet]
        [Route("reserve")]
        public async Task<ActionResult<object>> GetPendingRequestFromBuffer(DaisyControlGetPendingMessagesRequestDto getPendingMessagesFromBufferRequestDto)
        {
            object response = await workflow.ExecuteAsync(getPendingMessagesFromBufferRequestDto);

            if (response == null)
            {
                return NotFound();
            }

            return response;
        }

        /// <summary>
        /// get message request from storage.
        /// </summary>
        [HttpGet]
        [Route("{messageId}")]
        public async Task<ActionResult<object>> GetMessage(DaisyControlGetMessageFromBufferRequestDto getMessageFromBufferRequestDto)
        {
            object response = await workflow.ExecuteAsync(getMessageFromBufferRequestDto);

            if (response == null)
            {
                return NotFound();
            }

            return response;
        }

        /// <summary>
        /// Add new message request to storage.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> AddMessage([FromBody] DaisyControlAddMessageToBufferRequestDto addMessageToBufferRequestDto)
        {
            object response = await workflow.ExecuteAsync(addMessageToBufferRequestDto);
            return response;
        }

        ///// <summary>
        ///// Update (full obj) User to storage.
        ///// </summary>
        //[HttpPut]
        //[Route("users/{userIdToUpdate}")]
        //public async Task<ActionResult<object>> UpdateUser([FromRoute]string UserIdToUpdate, DaisyControlUpdateUserRequestDto userRequest)
        //{
        //    // Validate request
        //    if(userRequest.Id != UserIdToUpdate)
        //    {
        //        throw new BadRequestWebApiException("d117c895-568d-4376-a785-466fcc8b6f7b", $"UserId [{UserIdToUpdate}] to update didn't match the provided body UserId [{userRequest.Id}].");
        //    }

        //    object response = await workflow.ExecuteAsync(userRequest);
        //    return response;
        //}

        ///// <summary>
        ///// Delete User from storage.
        ///// </summary>
        //[HttpDelete]
        //[Route("users/{userId}")]
        //public async Task<ActionResult<object>> DeleteUser(DaisyControlDeleteUserRequestDto userRequest)
        //{
        //    object response = await workflow.ExecuteAsync(userRequest);
        //    return response;
        //}
    }
}
