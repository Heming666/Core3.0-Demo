using DBModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IUserInfo
    {
        Task<IEnumerable<UserInfo>> GetAllUser(string name, int? age);
        Task<bool> Del(Guid id);
    }
}
