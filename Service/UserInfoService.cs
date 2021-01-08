using IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DBModel;
using System.Linq.Expressions;
using DBModel.Models;

namespace Service
{
    public class UserInfoService : BaseRepository<UserInfo>, IUserInfo
    {
        private EFCoreDbContext db;
        public UserInfoService()
        {
            db = new EFCoreDbContext();
        }

        public Task<bool> Del(Guid id)
        {
            return Task.Run(() =>
            {
              return  Delete(id);
            });
        }

        public Task<IEnumerable<UserInfo>> GetAllUser(string name, int? age)
        {
            return Task.Run(() =>
            {
                Expression<Func<UserInfo, bool>> where = p => true;
                if (!string.IsNullOrWhiteSpace(name)) where = p => p.Name.Contains(name);
                if (age.HasValue) where = p => p.Age == age;
                return GetList(where);
            });

        }
    }
}
