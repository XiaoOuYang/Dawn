using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    public class ExcelHelper
    {
        static readonly int EXCEL_MaxRow = 65535;
        /// <summary>
        /// 到处Excel根路径
        /// </summary>
        private static string EXCEL_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Document", "ToExcel");

        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>> _paramCache = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>>();


        #region Excel

        private string _filePath = string.Empty;
        private IWorkbook _book;
        /// <summary>
        /// 创建Excel文件
        /// </summary>
        /// <param name="fileName"></param>
        public ExcelHelper(string fileName)
        {
            if (_book != null)
                return;

            string saveFileName = GetSaveFileName(fileName);
            _filePath = GetFilePath(saveFileName);
            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            }

            using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (_filePath.ToLower().EndsWith(".xlsx")) //2007版本以上
                    _book = new XSSFWorkbook();
                else if (_filePath.ToLower().IndexOf(".xls") > 0) //  2003 以下
                    _book = new HSSFWorkbook();
                else
                    _book = new XSSFWorkbook();
            }
        }


        /// <summary>
        /// 添加Sheet。如果列表超出最大行数会自动分Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public ExcelHelper AddSheet<T>(ICollection<dynamic> list, string sheetName)
        {
            if (list.Count < EXCEL_MaxRow)
                DynamicListWriteToSheet<T>(list, 0, list.Count, _book, sheetName);
            else
            {
                int page = list.Count / EXCEL_MaxRow;
                for (int i = 0; i < page; i++)
                {
                    int start = i * EXCEL_MaxRow;
                    int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                    DynamicListWriteToSheet<T>(list, start, end, _book, sheetName + i.ToString());
                }
                int lastPageItemCount = list.Count % EXCEL_MaxRow;
                DynamicListWriteToSheet<T>(list, list.Count - lastPageItemCount, lastPageItemCount, _book, sheetName + page.ToString());
            }

            return this;
        }

        /// <summary>
        /// 添加Sheet。如果列表超出最大行数会自动分Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public ExcelHelper AddSheet<T>(ICollection<T> list, string sheetName) where T : class
        {
            if (list.Count < EXCEL_MaxRow)
                ListWriteToSheet<T>(list, 0, list.Count, _book, sheetName);
            else
            {
                int page = list.Count / EXCEL_MaxRow;
                for (int i = 0; i < page; i++)
                {
                    int start = i * EXCEL_MaxRow;
                    int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                    ListWriteToSheet<T>(list, start, end, _book, sheetName + i.ToString());
                }
                int lastPageItemCount = list.Count % EXCEL_MaxRow;
                ListWriteToSheet<T>(list, list.Count - lastPageItemCount, lastPageItemCount, _book, sheetName + page.ToString());
            }

            return this;
        }

        /// <summary>
        /// 返回磁盘路径
        /// </summary>
        /// <returns></returns>
        public string Write()
        {
            using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                _book.Write(fs);
            }

            return _filePath;
        }

        public void Write(out string filePath)
        {
            using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                _book.Write(fs);
            }
            filePath = _filePath;
        }

        //public Tuple<string, string> Write()
        //{
        //    using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //    {
        //        _book.Write(fs);
        //    }
        //    return new Tuple<string, string>(_filePath, GetFileName(_filePath));
        //}


        #endregion



        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// Dynamic To Excel
        /// </summary>
        /// <typeparam name="Dynamic"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string DynamicListToExcel<T>(ICollection<dynamic> list, string sheetName, string fileName) where T : class
        {
            string saveFileName = GetSaveFileName(fileName);
            var filePath = GetFilePath(saveFileName);
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook book;
                if (filePath.ToLower().EndsWith(".xlsx")) //2007版本以上
                    book = new XSSFWorkbook();
                else if (filePath.ToLower().IndexOf(".xls") > 0) //  2003 以下
                    book = new HSSFWorkbook();
                else
                    book = new XSSFWorkbook();

                if (list.Count < EXCEL_MaxRow)
                    DynamicListWriteToSheet<T>(list, 0, list.Count, book, sheetName);
                else
                {
                    int page = list.Count / EXCEL_MaxRow;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * EXCEL_MaxRow;
                        int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                        DynamicListWriteToSheet<T>(list, start, end, book, sheetName + i.ToString());
                    }
                    int lastPageItemCount = list.Count % EXCEL_MaxRow;
                    DynamicListWriteToSheet<T>(list, list.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString());
                }
                book.Write(fs);
            }

            return filePath;

        }

        private static void DynamicListWriteToSheet<T>(ICollection<dynamic> list, int startRow, int endRow, IWorkbook book, string sheetName)
        {
            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyDic = list.ElementAt(0) as IDictionary<string, object>;
            for (int i = 0; i < propertyDic.Keys.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string name = propertyDic.Keys.ElementAt(i);
                var propertyInfo = properties.FirstOrDefault(q => q.Name == name);
                if (propertyInfo != null)
                    name = GetPropertyName(propertyInfo);
                cell.SetCellValue(name);
            }

            int rowIndex = 1;
            for (int i = startRow; i < endRow; i++)
            {
                var row = list.ElementAt(i) as IDictionary<string, object>;

                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < propertyDic.Keys.Count; j++)
                {
                    var val = row.Values.ElementAt(j);
                    excelRow.CreateCell(j).SetCellValue(val == null ? string.Empty : val.ToString());
                }
            }
        }



        /// <summary>
        /// List To  Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ListToExcel<T>(ICollection<T> list, string sheetName, string fileName)
        {
            string saveFileName = GetSaveFileName(fileName);
            var filePath = GetFilePath(saveFileName);


            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            CreateCache<T>();

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook book;
                if (filePath.ToLower().EndsWith(".xlsx")) //2007版本以上
                    book = new XSSFWorkbook();
                else if (filePath.ToLower().IndexOf(".xls") > 0) //  2003 以下
                    book = new HSSFWorkbook();
                else
                    book = new XSSFWorkbook();

                if (list.Count < EXCEL_MaxRow)
                    ListWriteToSheet(list, 0, list.Count, book, sheetName);
                else
                {
                    int page = list.Count / EXCEL_MaxRow;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * EXCEL_MaxRow;
                        int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                        ListWriteToSheet(list, start, end, book, sheetName + i.ToString());
                    }
                    int lastPageItemCount = list.Count % EXCEL_MaxRow;
                    ListWriteToSheet(list, list.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString());
                }
                book.Write(fs);
            }

            return filePath;

        }

        private static void ListWriteToSheet<T>(ICollection<T> list, int startRow, int endRow, IWorkbook book, string sheetName)
        {

            Dictionary<PropertyInfo, string> propertyDic = new Dictionary<PropertyInfo, string>();
            _paramCache.TryGetValue(typeof(T), out propertyDic);
            bool isCouvert = false;

            isCouvert = typeof(T).GetInterfaces().Contains(typeof(IConvert));

            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);

            for (int i = 0; i < propertyDic.Values.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string val = propertyDic.Values.ElementAt(i);
                cell.SetCellValue(val);
            }

            int rowIndex = 1;
            for (int i = startRow; i < endRow; i++)
            {
                var row = list.ElementAt(i);
                if (isCouvert)
                    (row as IConvert).ConvertText();

                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < propertyDic.Keys.Count; j++)
                {
                    var p = propertyDic.Keys.ElementAt(j);
                    var val = p.FastGetValue2(row);
                    excelRow.CreateCell(j).SetCellValue(val == null ? string.Empty : val.ToString());
                }
            }
        }


        private static string GetPropertyName(PropertyInfo property)
        {
            string name = string.Empty;
            DescriptionExcelAttribute deAttribute = null;
            if (property.IsDefined(typeof(DescriptionExcelAttribute), false))
                deAttribute = property.GetCustomAttribute<DescriptionExcelAttribute>();

            if (deAttribute != null)
                name = deAttribute.Description;

            if (string.IsNullOrEmpty(name))
                name = property.Name;

            return name;
        }

        private static void CreateCache<T>()
        {
            if (_paramCache.ContainsKey(typeof(T)))
                return;


            PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<PropertyInfo, string> propertyInfos = new Dictionary<PropertyInfo, string>();
            DescriptionExcelAttribute deAttribute = null;

            foreach (var property in propertys)
            {
                deAttribute = null;
                string name = string.Empty;
                if (property.IsDefined(typeof(DescriptionExcelAttribute), false))
                {
                    deAttribute = property.GetCustomAttribute<DescriptionExcelAttribute>();
                }

                if (deAttribute != null && deAttribute.ToExcel == false)
                    continue;

                if (deAttribute != null)
                    name = deAttribute.Description;

                if (string.IsNullOrEmpty(name))
                    name = property.Name;
                propertyInfos.Add(property, name);
            }

            _paramCache.TryAdd(typeof(T), propertyInfos);

        }

        /// <summary>
        /// 将DataTable转换为excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns>保存的文件名称包含扩展名</returns>
        public static string DataTableToExcel(DataTable dt, string sheetName, string fileName)
        {
            string saveFileName = GetSaveFileName(fileName);
            var filePath = GetFilePath(saveFileName);


            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook book;
                if (filePath.ToLower().EndsWith(".xlsx")) //2007版本以上
                    book = new XSSFWorkbook();
                else if (filePath.ToLower().IndexOf(".xls") > 0) //  2003 以下
                    book = new HSSFWorkbook();
                else
                    book = new XSSFWorkbook();

                if (dt.Rows.Count < EXCEL_MaxRow)
                    DataWriteToSheet(dt, 0, dt.Rows.Count - 1, book, sheetName);
                else
                {
                    int page = dt.Rows.Count / EXCEL_MaxRow;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * EXCEL_MaxRow;
                        int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow - 1;
                        DataWriteToSheet(dt, start, end, book, sheetName + i.ToString());
                    }
                    int lastPageItemCount = dt.Rows.Count % EXCEL_MaxRow;
                    DataWriteToSheet(dt, dt.Rows.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString());
                }
                book.Write(fs);
            }

            return saveFileName;

        }
        /// <summary>
        /// 保存的文件名称
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetSaveFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            if (fileName.ToLower().IndexOf(".xls") <= 0)
                fileName += ".xlsx";

            fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);

            return fileName;
        }

        private static string GetFilePath(string fileName)
        {
            string filePath = fileName;
            if (string.IsNullOrWhiteSpace(filePath))
                filePath = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (filePath.ToLower().IndexOf(".xls") > 0)
                filePath = Path.Combine(EXCEL_PATH, filePath);
            else
                filePath = Path.Combine(EXCEL_PATH, filePath + ".xlsx");

            return filePath;
        }

        private static void DataWriteToSheet(DataTable dt, int startRow, int endRow, IWorkbook book, string sheetName)
        {
            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string val = dt.Columns[i].Caption ?? dt.Columns[i].ColumnName;
                cell.SetCellValue(val);
            }
            int rowIndex = 1;
            for (int i = startRow; i <= endRow; i++)
            {
                DataRow dtRow = dt.Rows[i];
                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < dtRow.ItemArray.Length; j++)
                {
                    excelRow.CreateCell(j).SetCellValue(dtRow[j].ToString());
                }
            }

        }


        /// <summary>
        /// 将excel中的数据导入到DataTable中 
        ///  默认路径为/Document/ToExcel/
        /// </summary>
        /// <param name="fileName">文件名称比如：a.xlsx</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(string fileName, bool isFirstRowColumn = false, string sheetName = "")
        {

            var filePath = GetFilePath(fileName);

            ISheet sheet = null;
            DataTable data = new DataTable();
            IWorkbook workbook;
            int startRow = 0;
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (fileName.IndexOf(".xlsx") > 0)
                    {
                        try
                        {
                            workbook = new XSSFWorkbook(fs);
                        }
                        catch (Exception)
                        {
                            workbook = new HSSFWorkbook(fs);
                        }

                    }
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    else
                        workbook = new XSSFWorkbook(fs);

                    if (!string.IsNullOrWhiteSpace(sheetName))
                    {
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                        if (isFirstRowColumn)
                        {
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }

                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (!string.IsNullOrWhiteSpace(cellValue))
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }

                        //最后一行的标号
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {

                            IRow row = sheet.GetRow(i);
                            if (row == null) continue; //没有数据的行默认是null　

                            DataRow dataRow = data.NewRow();

                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            data.Rows.Add(dataRow);
                        }
                    }

                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            var entityType = entitys[0].GetType();
            var entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            var dt = new DataTable();
            foreach (var t in entityProperties)
            {
                dt.Columns.Add(t.Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                var entityValues = new object[entityProperties.Length];
                for (var i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].FastGetValue2(entity);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }


    }
}

