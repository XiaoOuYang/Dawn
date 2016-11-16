using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dawn.Tests.Consoles.Common
{
    public static class MvcContextMockFactory
    {
        //MvcContextMockFactory.CreateControllerContext(homeController, "/Home/index", "get", "home", "{controller}/{action}", null);


        private static ControllerContext controllerContext = null;
        /// <summary>
        /// 创建ControllerContext
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <returns></returns>
        public static ControllerContext CreateControllerContext(Controller controller)
        {
            controllerContext = new ControllerContext
              (
             CreateHttpContext(),
             new RouteData(),
              controller);
            return controllerContext;
        }

        /// <summary>
        /// 创建ControllerContext
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="contextBase">httpContextBase</param>
        /// <returns></returns>
        public static ControllerContext CreateControllerContext(Controller controller, HttpContextBase contextBase)
        {
            controllerContext = new ControllerContext
              (
             contextBase,
             new RouteData(),
              controller);
            return controllerContext;
        }


        /// <summary>
        /// 创建ControllerContext
        /// </summary>
        /// <param name="controller">controller</param>
        /// <param name="url">访问的url地址</param>
        /// <param name="httpMethod">访问的方法（get,post）</param>
        /// <param name="name">路由名称</param>
        /// <param name="pattern">路由格式</param>
        /// <param name="obj">路由默认值</param>
        /// <returns></returns>
        public static ControllerContext CreateControllerContext(Controller controller, string url, string httpMethod, string name, string pattern, string obj)
        {
            controllerContext = new ControllerContext
               (
               CreateHttpContext(),
               GetRouteData(url, httpMethod, name, pattern, obj),
               controller);
            return controllerContext;
        }

        /// <summary>
        /// 创建ControllerContext
        /// </summary>
        /// <param name="controller">controller</param>
        /// <param name="contextBase">HttpContextBase</param>
        /// <param name="url">访问的url地址</param>
        /// <param name="httpMethod">访问的方法（get,post）</param>
        /// <param name="name">路由名称</param>
        /// <param name="pattern">路由格式</param>
        /// <param name="obj">路由默认值</param>
        /// <returns></returns>
        public static ControllerContext CreateControllerContext(Controller controller, HttpContextBase contextBase, string url, string httpMethod, string name, string pattern, string obj)
        {
            controllerContext = new ControllerContext
               (
               contextBase,
               GetRouteData(url, httpMethod, name, pattern, obj),
               controller);
            return controllerContext;
        }

        /// <summary>
        /// 创建HttpContextBase
        /// </summary>
        /// <returns></returns>
        public static HttpContextBase CreateHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            response
                .Setup(rsp => rsp.ApplyAppPathModifier(Moq.It.IsAny<string>()))
                .Returns((string s) => s);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            //request.Setup(re => re.Url).Returns();
            return context.Object;
        }

        #region Private Method
        private static HttpContextBase CreateHttpContext(string url, string httpMethod)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            response
                .Setup(rsp => rsp.ApplyAppPathModifier(Moq.It.IsAny<string>()))
                .Returns((string s) => s);

            request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns(url);
            request.Setup(req => req.HttpMethod).Returns(httpMethod);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            return context.Object;
        }

        private static RouteData GetRouteData(string url, string httpMethod, string name, string pattern, string obj)
        {
            RouteTable.Routes.MapRoute(name, pattern, obj);
            var routeData =
                RouteTable.Routes.
                GetRouteData(CreateHttpContext(url, httpMethod));
            return routeData;
        }
        #endregion

    }
}
