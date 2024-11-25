using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Response;

namespace DaisyControl_AI.Storage.Dtos.Requests
{
    /// <summary>
    /// Represent a request to add a new user to the database.
    /// </summary>
    public class DaisyControlAddUserRequestDto : DaisyControlUserResponseDto, IDataItem
    {
    }
}
