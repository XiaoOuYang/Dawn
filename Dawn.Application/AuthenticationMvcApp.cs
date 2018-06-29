using Dawn.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Dawn.Application
{

    public class AuthenticationMvcApp : IAuthenticationApp
    {
        private readonly IContextService _contextService;
        public AuthenticationMvcApp(
            IContextService contextService)
        {
            _contextService = contextService;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userName"></param>
        /// <param name="createPersistentCookie"></param>
        public void SignIn(int Id, string userName, bool createPersistentCookie = true)
        {
            var now = DateTime.Now;

            var ticket = new FormsAuthenticationTicket(
                1,
                userName,
                now,
                DateTime.MaxValue,
                createPersistentCookie,
                Id.ToString(),
                FormsAuthentication.FormsCookiePath);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);


            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);

            _contextService.ClearCurrentUser();

        }
        public void SignOut()
        {
            int userId = _contextService.CurrentAccount.AccountId;

            FormsAuthentication.SignOut();

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1),
            };

            HttpContext.Current.Response.Cookies.Add(cookie);

            _contextService.ClearCurrentUser();

        }



    }
}
