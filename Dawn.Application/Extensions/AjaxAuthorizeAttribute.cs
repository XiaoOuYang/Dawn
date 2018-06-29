using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dawn.Application.Extensions
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        errorMsg = "您还没有登陆，请先登陆后再进行操作。"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return;
            }
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
