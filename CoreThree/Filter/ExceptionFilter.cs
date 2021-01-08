using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreThree.Filter
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider _moprovider;
        public ExceptionFilter(IModelMetadataProvider moprovider)
        {
            this._moprovider = moprovider;
        }
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Error(context.Exception,"错误");
            if (!context.ExceptionHandled)//如果异常没有被处理过
            {
                WriteLog.AddLog(JsonConvert.SerializeObject(context.Exception.Message));
                string controllerName = (string)context.RouteData.Values["controller"];
                string actionName = (string)context.RouteData.Values["action"];
                //string msgTemplate =string.Format( "在执行controller[{0}的{1}]方法时产生异常",controllerName,actionName);//写入日志
                
                if (this.IsAjaxRequest(context.HttpContext.Request))
                {

                    context.Result = new JsonResult(new
                    {
                        Result = false,
                        PromptMsg = "系统出现异常，请联系管理员",
                        DebugMessage = context.Exception.Message
                    });
                }
                else
                {
                    var result = new ViewResult { ViewName = "Error" };
                    result.ViewData = new ViewDataDictionary(_moprovider, context.ModelState);
                    result.ViewData.Add("Execption", context.Exception);
                    context.Result = result;
                }

;
            }
        }
        //判断是否为ajax请求
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
