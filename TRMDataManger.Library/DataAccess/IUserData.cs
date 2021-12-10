using System.Collections.Generic;
using TRMDataManger.Library.Models;

namespace TRMDataManger.Library.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string Id);
    }
}