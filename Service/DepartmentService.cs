using DBModel;
using DBModel.Models;
using IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Service
{
    public class DepartmentService : BaseRepository<Department>, IDearptment
    {
        public Task AddDept(Department entity)
        {
            Add(entity);
            return Task.CompletedTask;
        }

        public Task<bool> DelDept(Guid deptId)
        {
            return Delete(deptId);
        }

        public Task<IEnumerable<Department>> GetAll()
        {
            return Task.Run(() => GetList());
        }

        public Task<bool> HasEmployee(Guid deptId)
        {
            return Task.Run(() => {
                BaseRepository<UserInfo> repository = new BaseRepository<UserInfo>();
                int count = repository.Count(p => p.DeptId == deptId).Result;
                return count > 0;
            });
        }
    }
}
