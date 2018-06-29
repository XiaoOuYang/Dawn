using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.WebPages;
using Dawn.Infrastructure.Interfaces.Extensions;
namespace Dawn.Application.Extensions
{
    /// <summary>
    /// HtmlHelper 扩展类
    /// </summary>
    public static class HtmlHelperExt
    {

        /// <summary>
        /// 把Post提交的验证Token写入Cookie
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static HtmlString PostVerficationToken(this HtmlHelper html)
        {
            SessionPageTokenView.SetVerficationToken();
            return new HtmlString(string.Empty);
        }



        #region 缓存


        public static HtmlString Cache(this HtmlHelper htmlHelper, string cacheKey, DateTime absoluteExpiration, Action action)
        {
            return htmlHelper.Cache(cacheKey, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, action);
        }
        /// <summary>
        /// 对页面中指定字符串进行缓存
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheDependencies"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HtmlString Cache(this HtmlHelper htmlHelper, string cacheKey, CacheDependency cacheDependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, Action action)
        {

            var cache = htmlHelper.ViewContext.HttpContext.Cache;
            var content = cache.Get(cacheKey) as string;

            if (content == null)
            {
                var writer = htmlHelper.ViewContext.Writer;
                var record = new StringBuilder();
                var recordWriter = new RecordWriter(writer, record);
                //使用有记录功能的Writer
                htmlHelper.ViewContext.Writer = recordWriter;

                action();
                //得到产生的Writer
                content = record.ToString();
                //换成原始的writer
                htmlHelper.ViewContext.Writer = writer;

                if (string.IsNullOrWhiteSpace(content))
                {
                    content = string.Empty;
                }

                cache.Insert(cacheKey, content, cacheDependencies, absoluteExpiration, slidingExpiration);

                return new HtmlString(string.Empty);
            }

            return new HtmlString(content);
        }


        /// <summary>
        /// 对页面中指定字符串进行缓存
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheDependencies">缓存依附对象，可以为null</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="slidingExpiration">相对本次过期时间</param>
        /// <param name="func">要缓存对象</param>
        /// <returns></returns>
        public static IHtmlString Cache(this HtmlHelper htmlHelper, string cacheKey, CacheDependency cacheDependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, Func<object, IHtmlString> func)
        {
            var cache = htmlHelper.ViewContext.HttpContext.Cache;
            var content = cache.Get(cacheKey) as string;
            if (content == null)
            {

                content = func.Invoke(null).ToHtmlString();
                cache.Insert(cacheKey, content, cacheDependencies, absoluteExpiration, slidingExpiration);
            }

            return new HelperResult(writer =>
            {
                writer.Write(content);
            });
        }



        /// <summary>
        /// 对页面中指定字符串进行缓存
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheDependencies">缓存依附对象，可以为null</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="slidingExpiration">相对本次过期时间</param>
        /// <param name="func">要缓存对象</param>
        /// <returns></returns>
        public static HelperResult Cache(this HtmlHelper htmlHelper, string cacheKey, CacheDependency cacheDependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, Func<object, HelperResult> func)
        {
            var cache = htmlHelper.ViewContext.HttpContext.Cache;
            var content = cache.Get(cacheKey) as string;

            if (content == null)
            {
                content = func.Invoke(null).ToHtmlString();
                cache.Insert(cacheKey, content, cacheDependencies, absoluteExpiration, slidingExpiration);
            }

            return new HelperResult(writer =>
            {
                writer.Write(content);
            });
        }

        /// <summary>
        /// 对页面中指定字符串进行缓存
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="slidingExpiration">相对本次过期时间</param>
        /// <param name="func">要缓存对象</param>
        /// <returns></returns>
        public static HelperResult Cache(this HtmlHelper htmlHelper, string cacheKey, DateTime absoluteExpiration, TimeSpan slidingExpiration, Func<object, HelperResult> func)
        {
            return Cache(htmlHelper, cacheKey, null, absoluteExpiration, slidingExpiration, func);
        }

        /// <summary>
        /// 对页面中指定字符串进行缓存
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="func">要缓存对象</param>
        /// <returns></returns>
        public static HelperResult Cache(this HtmlHelper htmlHelper, string cacheKey, DateTime absoluteExpiration, Func<object, HelperResult> func)
        {
            return Cache(htmlHelper, cacheKey, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, func);
        }


        #endregion


        #region URl

        /// <summary>
        /// 转换成全路径 https://...
        /// </summary>
        /// <param name="urlHelp"></param>
        /// <param name="path">路径</param>
        /// <param name="defaultVal">为空事默认值</param>
        /// <param name="domain">默认是当前域名，指定域名 如：wx.233.com/system</param>
        /// <returns></returns>
        public static string ClientPath(this UrlHelper urlHelp, string pathString, string defaultVal = "#", string domain = "")
        {
            string path = pathString;
            string tDefaultVal = defaultVal;
            string tDomain = domain;

            if (string.IsNullOrWhiteSpace(path)) return tDefaultVal;

            if (path.StartsWith("#")) return path;

            if (path.StartsWith("http") || path.StartsWith("//"))
                return path;

            if (!path.StartsWith("~"))
            {
                if (path.StartsWith("/"))
                    path = urlHelp.Content("~" + path);
                else
                    path = urlHelp.Content("~/" + path);
            }
            else
                path = urlHelp.Content(path);

            if (string.IsNullOrEmpty(tDomain))
                tDomain = urlHelp.RequestContext.HttpContext.Request.Url.Authority;

            if (!tDomain.EndsWith("/"))
                tDomain = tDomain + "/";

            path = tDomain + path.TrimStart('/');

            return "https://" + path;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlHelp"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string LocalUrl(this UrlHelper urlHelp, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "Javascript:void(0);";
            }


            if (urlHelp.RequestContext.HttpContext.Request.IsLocal)
                return urlHelp.Content("~" + url.Replace("/boss_report", string.Empty));

            if (url.StartsWith("~"))
                return urlHelp.Content(url);

            return urlHelp.Content("~" + url);
        }


        #endregion

         
    }
}
