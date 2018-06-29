
using Dawn.Infrastructure.Interfaces;
using System.Web;

namespace Dawn.Application.Extensions
{
    public class SessionPageTokenView
    {
        public const string PAGETOKENKEY = "SESSION_POSTPAGETOKEY";
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GeneratePageToken()
        {
            var tokenValue = HttpContext.Current.Session[PAGETOKENKEY] as string;
            if (tokenValue == null)
            {
                tokenValue = IdWorker.NewStringId();
                HttpContext.Current.Session[PAGETOKENKEY] = tokenValue;
            }

            return tokenValue;
        }


        /// <summary>
        /// 设置Token到Cookie
        /// </summary>
        public static void SetVerficationToken()
        {
            HttpContext.Current.Session.Remove(PAGETOKENKEY);
            HttpCookie cookie = new HttpCookie(PAGETOKENKEY, GeneratePageToken());
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        /// <summary>
        /// 验证Token是否匹配
        /// </summary>
        /// <returns></returns>
        public static bool VerficationToken()
        {
            string tokenValue = GeneratePageToken();
            HttpContext.Current.Session.Remove(PAGETOKENKEY);

            var cookie = HttpContext.Current.Request.Cookies[PAGETOKENKEY];
            if (cookie == null)
                return false;
            if (cookie.Value == tokenValue)
            {
                return true;
            }
            return false;
        }


    }
}
