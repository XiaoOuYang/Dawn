using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    /// <summary>
    /// 导出Excel 使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class DescriptionExcelAttribute : Attribute
    {
        public DescriptionExcelAttribute()
        {
            ToExcel = false;
            Description = string.Empty;
        }

        public DescriptionExcelAttribute(string description, bool toExcel = true)
        {
            Description = description;
            ToExcel = toExcel;
        }


        public string Description { get; set; }
        /// <summary>
        /// 是否要导出Execl
        /// </summary>
        public bool ToExcel { get; set; }
    }
}
