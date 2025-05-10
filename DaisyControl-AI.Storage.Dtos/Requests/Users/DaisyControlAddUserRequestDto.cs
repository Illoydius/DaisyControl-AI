using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Storage.Dtos.Requests.Users
{
    /// <summary>
    /// Represent a request to add a new user to the database.
    /// </summary>
    public class DaisyControlAddUserRequestDto : DaisyControlUserDto, IDataItem
    {
    }
}
