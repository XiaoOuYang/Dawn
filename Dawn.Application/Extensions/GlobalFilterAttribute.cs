using System.Web.Mvc;

namespace Dawn.Application.Extensions
{
    /// <summary>
    /// 全局过滤器
    /// </summary>
    public class GlobalFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在Action方法调用前使用，使用场景：如何验证登录等。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            
        }

        /// <summary>
        /// 在Action方法调用后，result方法调用前执行，使用场景：异常处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
           
        }

        /// <summary>
        /// 在result执行前发生(在view 呈现前)，使用场景：设置客户端缓存，服务器端压缩.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            
        }

        /// <summary>
        /// 在result执行后发生，使用场景：异常处理，页面尾部输出调试信息
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            ////检查是否设置了"页面模块标识"
            //if (!filterContext.IsChildAction && filterContext.Controller.ViewData["PageModuleTag"] == null)
            //{
            //    filterContext.HttpContext.Response.Write("请设置\"页面模块标识\"");
            //    filterContext.HttpContext.Response.End();
            //}
        }
    }
}
