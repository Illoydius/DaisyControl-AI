using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.Response.Users;

namespace DaisyControl_AI.Storage.Dtos.Requests
{
    /// <summary>
    /// Represent a request to update an existing user to the database.
    /// </summary>
    public class DaisyControlUpdateUserRequestDto: DaisyControlUserResponseDto, IDataItem
    {
    }
}
