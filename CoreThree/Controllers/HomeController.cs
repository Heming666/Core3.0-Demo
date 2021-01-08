using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IService;
using Microsoft.AspNetCore.Mvc;
using DBModel.Models;

namespace CoreThree.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserInfo _IUserInfo;
        private readonly IDearptment _deptService;
        public HomeController(IUserInfo userInfo,IDearptment dept)
        {
            _IUserInfo = userInfo;
            _deptService = dept;
        }
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="age">年龄</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string name,int? age)
        {
            var data =  await _IUserInfo.GetAllUser(name,age);
            return View(data);
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<IActionResult> DelUser(Guid userId)
        {
            var data = await _IUserInfo.Del(userId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddUser()
        {
            UserInfo userInfo = new UserInfo() { Id = new Guid() };
            var deptList = _deptService.GetAll();
            ViewBag.DeptList = deptList;
            return View(userInfo);
        }
    }
}