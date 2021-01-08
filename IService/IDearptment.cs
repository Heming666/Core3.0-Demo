using DBModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IDearptment
    {
        Task<IEnumerable<Department>> GetAll();
        Task AddDept(Department entity);
        Task<bool> HasEmployee(Guid deptId);
        Task<bool> DelDept(Guid deptId);
    }
}
