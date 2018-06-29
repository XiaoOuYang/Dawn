using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dawn.Infrastructure.Interfaces.Extensions.Dynamic;

namespace Dawn.Infrastructure.Interfaces
{
    public class DynamicList
    {
        private readonly Dictionary<string, Delegate> _dicFunc;
        private readonly Dictionary<PropertyInfo, string> _dicPropertyNames;

        public DynamicList()
        {
            _dicFunc = new Dictionary<string, Delegate>();
            _dicPropertyNames = new Dictionary<PropertyInfo, string>();
        }

        /// <summary>
        /// 动态Linq方式实现行转列
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="dimensionList">维度列</param>
        /// <param name="dynamicColumn">动态列</param>
        /// <param name="dynamicCoumnValue">动态列对应的值</param>
        /// <param name="isEnabledDimensionAlias">是否启用维度列的别名</param>
        /// <returns>行转列后数据</returns>
        public IList<dynamic> RowToColumn<T>(IEnumerable<T> list, List<string> dimensionList, string dynamicColumn, string dynamicCoumnValue, bool isEnabledDimensionAlias = false) where T : class
        {
            IEnumerable<T> List = list;
            string DynamicCoumnValue = dynamicCoumnValue;
            string DynamicColumn = dynamicColumn;
            List<string> DimensionList = dimensionList;

            //获取所有动态列
            var columnGroup = List.GroupBy(DynamicColumn, "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
            List<string> AllColumnList = new List<string>();
            foreach (var item in columnGroup)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    AllColumnList.Add(item.Key);
                }
            }

            var dictFunc = new Dictionary<string, Func<T, bool>>();
            foreach (var column in AllColumnList)
            {
                var func = Extensions.Dynamic.DynamicExpression.ParseLambda<T, bool>(string.Format("{0}==\"{1}\"", DynamicColumn, column)).Compile();
                dictFunc[column] = func;
            }

            //获取实体所有属性
            Dictionary<string, PropertyInfo> PropertyInfoDict = new Dictionary<string, PropertyInfo>();
            Type type = typeof(T);
            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var item in propertyInfos)
            {
                PropertyInfoDict[item.Name] = item;
            }

            Type dynamicCoumnValueType = PropertyInfoDict[DynamicCoumnValue].PropertyType;

            //分组
            var dataGroup = List.GroupBy(string.Format("new ({0})", string.Join(",", DimensionList)), "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
            List<dynamic> listResult = new List<dynamic>();
            IDictionary<string, object> itemObj = null;
            T vm2 = default(T);
            List<T> vm2List = new List<T>();
            foreach (var group in dataGroup)
            {
                itemObj = new ExpandoObject();
                var listVm = group.Select(e => e.Vm as T).ToList();
                //维度列赋值
                vm2 = listVm.FirstOrDefault();
                foreach (var key in DimensionList)
                {
                    string name = key;
                    if (isEnabledDimensionAlias)
                        name = GetPropertyName(PropertyInfoDict[key]);

                    itemObj[name] = PropertyInfoDict[key].GetValue(vm2);
                }

                foreach (var column in AllColumnList)
                {
                    vm2List = listVm.Where(dictFunc[column]).ToList();

                    if (dynamicCoumnValueType == typeof(decimal))
                        itemObj[column] = vm2List.Select(GetFunc<T, decimal>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(decimal?))
                        itemObj[column] = vm2List.Select(GetFunc<T, decimal?>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(double))
                        itemObj[column] = vm2List.Select(GetFunc<T, double>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(double?))
                        itemObj[column] = vm2List.Select(GetFunc<T, double?>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(float))
                        itemObj[column] = vm2List.Select(GetFunc<T, float>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(float?))
                        itemObj[column] = vm2List.Select(GetFunc<T, float?>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(int))
                        itemObj[column] = vm2List.Select(GetFunc<T, int>(DynamicCoumnValue)).Sum();
                    else if (dynamicCoumnValueType == typeof(int?))
                        itemObj[column] = vm2List.Select(GetFunc<T, int?>(DynamicCoumnValue)).Sum();
                    else
                        itemObj[column] = string.Join(",", vm2List.Select(GetFunc<T, string>(DynamicCoumnValue)));
                }

                listResult.Add(itemObj);
            }
            return listResult;

        }

        private Func<T, TResult> GetFunc<T, TResult>(string expressions)
        {
            string expression = expressions;
            Delegate func;
            if (!_dicFunc.TryGetValue(expression, out func))
            {
                func = DynamicExpression.ParseLambda<T, TResult>(expression).Compile();
                _dicFunc.Add(expression, func);
            }
            return (Func<T, TResult>)func;
        }

        /// <summary>
        /// 取属性的别名
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetPropertyName(PropertyInfo property)
        {
            string name = string.Empty;
            if (!_dicPropertyNames.TryGetValue(property, out name))
            {
                DescriptionExcelAttribute deAttribute = null;
                if (property.IsDefined(typeof(DescriptionExcelAttribute), false))
                    deAttribute = property.GetCustomAttribute<DescriptionExcelAttribute>();

                if (deAttribute != null)
                    name = deAttribute.Description;

                if (string.IsNullOrEmpty(name))
                    name = property.Name;
            }

            return name;
        }


    }
}
