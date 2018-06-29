using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToYMD(this DateTime? dt, bool isSeparation = true)
        {
            if (dt == null) return string.Empty;
            var text = dt.Value.ToString("yyyy-MM-dd");
            if (isSeparation == false)
                text = text.Replace("-", string.Empty);
            return text;
        }

        public static string ToYMDHMS(this DateTime? dt)
        {
            if (dt == null) return string.Empty;
            return dt.Value.ToString("yyyy-MM-dd hh:mm:ss");
        }

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToYMD(this DateTime dt, bool isSeparation = true)
        {
            var text = dt.ToString("yyyy-MM-dd");
            if (isSeparation == false)
                text = text.Replace("-", string.Empty);
            return text;
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToYMDHMS(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd hh:mm:ss");
        }

    }
}