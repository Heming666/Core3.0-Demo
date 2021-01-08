using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel.Models;
using IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreThree.Controllers
{
    public class DepartmentController : Controller
    {
        private IDearptment _deptService;
        public DepartmentController(IDearptment deptService)
        {
            _deptService = deptService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Department> deptList = await _deptService.GetAll();
            return View(deptList);
        }
        public IActionResult AddDepart()
        {
                Department dept = new Department() { DeptId = new Guid() };
                return View(dept);
        }

        [HttpPost]
        public async  Task<IActionResult> AddDepart(Department entity)
        {
            if (ModelState.IsValid)
            {
              await  _deptService.AddDept(entity);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DelDept(Guid deptId)
        {
            if (await _deptService.HasEmployee(deptId))
                return Content(JsonConvert.SerializeObject( new { Code = -1, Msg = "部门底下有员工，无法删除" }));
            if (await _deptService.DelDept(deptId))
                return Content(JsonConvert.SerializeObject(new { Code = 0, Msg = "操作成功" }));
            else
                return Content(JsonConvert.SerializeObject(new { Code = -1, Msg = "操作失败" }));
        }
    }
}