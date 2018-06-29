using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Net;
using Dawn.Infrastructure.Interfaces.Extensions;
using Dawn.Infrastructure.Interfaces;
using Dawn.Application.Interface;

namespace Dawn.Application.Extensions
{
    /// <summary>
    /// 全局验证过滤器
    /// </summary>
    public class GlobalAuthorizeAttribute : AuthorizeAttribute
    {
        public GlobalAuthorizeAttribute()
        {
            this.IsController = false;
            PageModuleTag = string.Empty;
            Permissions = string.Empty;
        }

        public GlobalAuthorizeAttribute(bool isController)
        {
            this.IsController = isController;
            this.Permissions = string.Empty;
            PageModuleTag = string.Empty;
        }

        /// <summary>
        /// 是否为控制器 (默认为False)
        /// </summary>
        public bool IsController { get; set; }
        /// <summary>
        /// 页面模块标识
        /// </summary>
        public string PageModuleTag { get; set; }
        /// <summary>
        /// Action的功能权限多个权限用','分隔 
        /// <para>例如：Add,Del,Update</para>
        /// </summary>
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            //检查是否登录
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;


            return base.AuthorizeCore(httpContext);
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
          
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string pageModuleTag = PageModuleTag;
                //赋值  默认值是否为 控制器名称？
                filterContext.Controller.ViewBag.PageModuleTag = pageModuleTag;

                //是否为公用页面
                bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);


                var passed = VerifyIsSetAuthorizeAttribute(filterContext, skipAuthorization);

                //当时Action时才走具体的权限过滤
                if (!IsController && passed)
                {
                    VerifyPageAuthorize(filterContext);
                }
            }
            else
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                string returnUrl = urlHelper.Action("index", "Login", new { area = "" });
             
                filterContext.Result = new RedirectResult(returnUrl);

            }

            base.OnAuthorization(filterContext);

        }

        /// <summary>
        /// 验证是否编写了 GlobalAuthorize 属性 或是否设置了 PageModuleTag
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="skipAuthorization"></param>
        private bool VerifyIsSetAuthorizeAttribute(AuthorizationContext filterContext, bool skipAuthorization)
        {
            bool isPassed = true;
            if (PageModuleTag != "All")
            {
                if (IsController)
                {
                    var isAuthorize = filterContext.ActionDescriptor.IsDefined(typeof(GlobalAuthorizeAttribute),
                     inherit: true);
                    if (!isAuthorize && !skipAuthorization && !filterContext.IsChildAction)
                    {
                        filterContext.Result = new ContentResult()
                        {
                            Content = "请在Action上设置\"页面模块标识\"过滤器，[GlobalAuthorize(PageModuleTag = \"模块标识\")]"
                        };
                        isPassed = false;
                    }

                }
                else
                {

                    if (PageModuleTag.IsNullOrEmpty() && !skipAuthorization && !filterContext.IsChildAction)
                    {
                        filterContext.Result = new ContentResult()
                        {
                            Content = "PageModuleTag不能为空，请在Action上设置\"页面模块标识\"过滤器，[GlobalAuthorize(PageModuleTag = \"模块标识\")]"
                        };
                        isPassed = false;
                    }
                }
            }
            return isPassed;
        }

        /// <summary>
        /// 验证页面权限 和 操作功能权限
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="skipAuthorization"></param>
        /// <returns></returns>
        private bool VerifyPageAuthorize(AuthorizationContext filterContext)
        {
            bool isPassed = true;

            var currentUser = IocContainer.Resolve<IContextService>().CurrentUser;
            if (currentUser == null)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                return false;
            }
            if (Permissions == null) Permissions = string.Empty;

            Permissions = Permissions.Trim().ToUpper();
            string[] permissions = Permissions.Split(',').Where(q => !string.IsNullOrEmpty(q)).ToArray();
            var urlHelper = new UrlHelper(filterContext.RequestContext);
            string returnUrl = urlHelper.Action("index", "Home", new { area = "" });

            var urlReferrer = filterContext.RequestContext.HttpContext.Request.UrlReferrer;
            if (urlReferrer != null)
                returnUrl = urlReferrer.AbsolutePath;

            //如果没有页面权限
            if (!currentUser.ManagerModelList.Any(q => PageModuleTag.Equals(q.Tag, StringComparison.OrdinalIgnoreCase)) && PageModuleTag != "All")
            {
                filterContext.Result = new ContentResult()
                {
                    Content = string.Format("无此页面权限。<a href='{0}'>返回</a>", returnUrl)
                };

                isPassed = false;

            }
            else
            {
                //需求是 当 permissions 值为空时不验证。
                if (permissions.Length > 0)
                {
                    //过滤Action的功能权限 permissions 是否有 Add 或 Update 等 权限
                    if (!IocContainer.Resolve<IContextService>().GetPagePermissionList(PageModuleTag).Any(q => q.Tag != null && permissions.Contains(q.Tag.ToUpper())))
                    {
                        filterContext.Result = new ContentResult()
                        {
                            Content = string.Format("无此功能操作权限。<a href='{0}'>返回</a>", returnUrl)
                        };
                        isPassed = false;
                    }
                }
            }

            return isPassed;
        }




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
