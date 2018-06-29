using Dawn.Infrastructure.Interfaces;
using System;
using System.Net;
using System.Web.Mvc;

namespace Dawn.Application.Extensions
{
    /// <summary>
    /// 请求处理异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HandleExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var errorMessage = GetErrorMessage(filterContext);
            //写日志
            IocContainer.Resolve<ILoggerFactory>().Create(this.GetType()).Error(errorMessage);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        errorMsg = errorMessage
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                if (filterContext.Exception != null && filterContext.Exception is TimeoutException)
                {
                    View = "TimeoutError";
                }
                base.OnException(filterContext);
            }
        }

        private static string GetErrorMessage(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                if (filterContext.Exception is TimeoutException)
                {
                    return "服务器处理请求超时";
                }
                return filterContext.Exception.Message;
            }
            return "服务器未知错误";
        }
    }
}