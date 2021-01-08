using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreThree.Filter
{
    public class AuthFilter : IAuthorizationFilter
    {
        //优先级1：权限过滤器：它在Filter Pipleline中首先运行，并用于决定当前用户是否有请求权限。如果没有请求权限直接返回
        public void OnAuthorization(AuthorizationFilterContext context)
        {

        }
    }
}
