using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IService;
using Microsoft.AspNetCore.Mvc;

namespace CoreThree.Controllers
{
    public class DateHelperController : Controller
    {
        private IDateService _dateService;
        public DateHelperController(IDateService dateService)
        {
            _dateService = dateService;
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public async Task< IActionResult> Index()
        {
            return await Task.Run(() =>
             {
                 return View();
             });
        }
    }
}
