using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    public interface IConvert
    {
        /// <summary>
        /// 把字段转换成前端显示值
        /// </summary>
        void ConvertText();

        /// <summary>
        /// 把字段值转换成真实值
        /// </summary>
        void ConvertValue();
    }
}
