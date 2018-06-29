using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces.Extensions
{
    public static class StringExtensions
    {



        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }


        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 隐藏手机号中间4位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HidePhone4(this string value)
        {
            if (value.IsNotNullOrWhiteSpace())
            {
                if (value.Length > 7)
                    return (value.Substring(0, 3) + "****" + value.Substring(7, value.Length - 7));
            }

            return value;
        }

        public static string TrimNull(this string value)
        {
            if (value == null) return "";
            return value.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] TrimSplit(this string value)
        {
            if (value == null) return null;
            return value.Trim().Split(',');
        }
    }
}
