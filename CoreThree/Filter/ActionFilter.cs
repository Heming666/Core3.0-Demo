using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreThree.Filter
{
    public class ActionFilter : ActionFilterAttribute
    {
        //优先级3：方法过滤器：它会在执行Action方法前后被调用。这个可以在方法中用来处理传递参数和处理方法返回结果。
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            var req = context.HttpContext.Request;

            ////  这句很重要，开启读取 否者下面设置读取为0会失败

            //req.EnableBuffering();


            //using (var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            //{
            //    var a =   reader.ReadToEndAsync();
            //    logger.Info("请求参数:"+ a.Result);
            //}

            //// 这里读取过body  Position是读取过几次  而此操作优于控制器先行 控制器只会读取Position为零次的

            //req.Body.Position = 0;
            ////List<object> paras = new List<object>();
            ////if (context.HttpContext.Request.Form !=null)
            ////{
            ////    foreach (var item in context.HttpContext.Request.Form.Keys)
            ////    {
            ////        paras.Add(context.HttpContext.Request.Form[item]);

            ////    }
            ////}
            string paras = string.Empty;
            if (context.ActionArguments != null && context.ActionArguments.Count > 0)
            {
                paras += $"  From : { JsonConvert.SerializeObject(context.ActionArguments)}";
            }
            if (req.QueryString.HasValue)
            {
                paras += $"  QueryString : { req.QueryString.Value}";
            }
            logger.Info("收到请求   "+paras);
        }
    }
}
