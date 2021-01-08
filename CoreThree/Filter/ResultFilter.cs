using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreThree.Filter
{
    public class ResultFilter : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
