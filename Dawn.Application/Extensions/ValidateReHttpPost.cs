using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dawn.Application.Extensions
{
    /// <summary>
    /// 验证表单重复提交
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateReHttpPostAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.HttpMethod.ToUpper().Equals("POST"))
            {
                if (!SessionPageTokenView.VerficationToken())
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult
                        {
                            Data = new
                            {
                                success = false,
                                errorMsg = "重复操作。"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NoContent);
                    }
                    //throw new Exception("错误，不可重复提交。");
                }
            }

        }



    }
}
