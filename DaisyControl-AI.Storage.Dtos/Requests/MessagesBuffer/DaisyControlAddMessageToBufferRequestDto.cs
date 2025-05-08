using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Response.Messages;

namespace DaisyControl_AI.Storage.Dtos.Requests.Messages
{
    /// <summary>
    /// Represent a new message to add to the database for processing.
    /// </summary>
    public class DaisyControlAddMessageToBufferRequestDto : DaisyControlMessageToBufferRequestDto, IDataItem
    {
    }
}
