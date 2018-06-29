using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dawn.Infrastructure.Interfaces.Extensions;

namespace Dawn.Infrastructure.Interfaces
{

    /// <summary>
    /// 创建Excel文件
    /// </summary>
    public class Excel
    {

        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>> _paramCache = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>>();
        static readonly int EXCEL_MaxRow = 65535;
        private readonly MemoryStream _fileStream;
        private string _fileName = string.Empty;
        private IWorkbook _book;
        private bool _isWrite;
        private Func<PropertyInfo, bool> _filterProperty;

        public Excel(string fileName)
        {

            if (_book != null)
                return;

            _isWrite = false;

            _fileStream = new MemoryStream();
            _fileName = GetSaveFileName(fileName);

            if (_fileName.ToLower().EndsWith(".xlsx")) //2007版本以上
                _book = new XSSFWorkbook();
            else if (_fileName.ToLower().IndexOf(".xls") > 0) //  2003 以下
                _book = new HSSFWorkbook();
            else
                _book = new XSSFWorkbook();

        }

        /// <summary>
        /// List转换成Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Excel ListToExcel<T>(ICollection<T> list, string sheetName, string fileName, Action<string, IWorkbook, ICell> mapCell = null)
        {
            var excel = new Excel(fileName).AddSheet(list, sheetName, mapCell);
            excel.Write();
            return excel;
        }

        /// <summary>
        /// 添加Sheet。如果列表超出最大行数会自动分Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="filterProperty">导出属性过滤.如果字段不需要导出 返回 False。</param>
        /// <returns></returns>
        public Excel AddSheet<T>(ICollection<T> list, string sheetName, Func<PropertyInfo, bool> filterProperty, Action<string, IWorkbook, ICell> mapCell = null)
        {
            _filterProperty = filterProperty;

            return AddSheet(list, sheetName, mapCell);
        }
        /// <summary>
        /// 添加Sheet。如果列表超出最大行数会自动分Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="filterProperty">导出属性过滤.如果字段不需要导出返回 False。</param>
        /// <returns></returns>
        public Excel AddSheet<T>(ICollection<dynamic> list, string sheetName, Func<PropertyInfo, bool> filterProperty, Action<string, IWorkbook, ICell> mapCell = null)
        {
            _filterProperty = filterProperty;

            return AddSheet<T>(list, sheetName, mapCell);
        }

        /// <summary>
        /// 添加Sheet。如果列表超出最大行数会自动分Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public Excel AddSheet<T>(ICollection<dynamic> list, string sheetName, Action<string, IWorkbook, ICell> mapCell = null)
        {

            if (_isWrite)
                throw new Exception("已完成文件流的写入，不可以再添加Sheet。");

            if (list.Count < EXCEL_MaxRow)
                DynamicListWriteToSheet<T>(list, 0, list.Count, _book, sheetName, mapCell);
            else
            {
                int page = list.Count / EXCEL_MaxRow;
                for (int i = 0; i < page; i++)
                {
                    int start = i * EXCEL_MaxRow;
                    int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                    DynamicListWriteToSheet<T>(list, start, end, _book, sheetName + i.ToString(), mapCell);
                }
                int lastPageItemCount = list.Count % EXCEL_MaxRow;
                DynamicListWriteToSheet<T>(list, list.Count - lastPageItemCount, lastPageItemCount, _book, sheetName + page.ToString(), mapCell);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public Excel AddSheet<T>(ICollection<T> list, string sheetName, Action<string, IWorkbook, ICell> mapCell = null)
        {
            if (_isWrite)
                throw new Exception("已完成文件流的写入，不可以再添加Sheet。");

            CreateCache<T>();

            if (list.Count < EXCEL_MaxRow)
                ListWriteToSheet<T>(list, 0, list.Count, _book, sheetName, mapCell);
            else
            {
                int page = list.Count / EXCEL_MaxRow;
                for (int i = 0; i < page; i++)
                {
                    int start = i * EXCEL_MaxRow;
                    int end = (i * EXCEL_MaxRow) + EXCEL_MaxRow;
                    ListWriteToSheet<T>(list, start, end, _book, sheetName + i.ToString(), mapCell);
                }
                int lastPageItemCount = list.Count % EXCEL_MaxRow;
                ListWriteToSheet<T>(list, list.Count - lastPageItemCount, lastPageItemCount, _book, sheetName + page.ToString(), mapCell);
            }
            return this;
        }

        protected void DynamicListWriteToSheet<T>(ICollection<dynamic> list, int startRow, int endRow, IWorkbook book, string sheetName, Action<string, IWorkbook, ICell> mapCell = null)
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

                mapCell?.Invoke(name, book, cell);
            }



            int rowIndex = 1;
            for (int i = startRow; i < endRow; i++)
            {
                var row = list.ElementAt(i) as IDictionary<string, object>;

                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < propertyDic.Keys.Count; j++)
                {
                    var val = row.Values.ElementAt(j);
                    var newCell = excelRow.CreateCell(j);
                    newCell.CellStyle = header.GetCell(j).CellStyle;
                    newCell.SetCellValue(val == null ? string.Empty : val.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <param name="book"></param>
        /// <param name="sheetName"></param>
        /// <param name="mapCell">string, ICell (列名称,Workbook对象,列对象)</param>
        protected void ListWriteToSheet<T>(ICollection<T> list, int startRow, int endRow, IWorkbook book, string sheetName, Action<string, IWorkbook, ICell> mapCell = null)
        {
            Dictionary<PropertyInfo, string> temPropertyDic = new Dictionary<PropertyInfo, string>();
            Dictionary<PropertyInfo, string> propertyDic = new Dictionary<PropertyInfo, string>();
            _paramCache.TryGetValue(typeof(T), out temPropertyDic);
            bool isCouvert = false;

            if (_filterProperty != null)
            {
                foreach (var property in temPropertyDic)
                {
                    if (_filterProperty.Invoke(property.Key))
                        propertyDic.Add(property.Key, property.Value);
                }
            }
            else
            {

                propertyDic = temPropertyDic;
            }

            isCouvert = typeof(T).GetInterfaces().Contains(typeof(IConvert));

            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);



            for (int i = 0; i < propertyDic.Values.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string val = propertyDic.Values.ElementAt(i);
                cell.SetCellValue(val);

                mapCell?.Invoke(val, book, cell);
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
                    var cell = excelRow.CreateCell(j);
                    cell.CellStyle = header.GetCell(j).CellStyle;
                    cell.SetCellValue(val == null ? string.Empty : val.ToString());
                }
            }
        }

        //写入流
        public MemoryStream Write()
        {
            if (_isWrite)
                return _fileStream;

            _book.Write(_fileStream);
            _isWrite = true;
            return _fileStream;
        }


        public byte[] GetFileBytes()
        {
            return Write().ToArray();
        }

        /// <summary>
        /// Excle文件下载名称
        /// </summary>
        /// <returns></returns>
        public string FileDownloadName
        {
            get
            {
                return _fileName;
            }
        }

        protected void CreateCache<T>()
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

        protected string GetPropertyName(PropertyInfo property)
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

        protected string GetSaveFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            if (fileName.ToLower().IndexOf(".xls") <= 0 && fileName.ToLower().IndexOf(".xlsx") <= 0)
                fileName += ".xlsx";

            fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);

            return fileName;
        }

    }



    public class ExcelRead : IDisposable
    {
        private readonly IWorkbook _book;
        private readonly string _fileName;
        private ISheet _sheet;
        /// <summary>
        /// sheet 的 列名对应下标词典
        /// </summary>
        private readonly Dictionary<string, int> _dicColumnName;

        public ExcelRead(string readFileName, Stream fileStream)
        {
            _fileName = readFileName;
            _dicColumnName = new Dictionary<string, int>();

            if (_fileName.ToLower().EndsWith(".xlsx")) //2007版本以上
                _book = new XSSFWorkbook(fileStream);
            else if (_fileName.ToLower().IndexOf(".xls") > 0) //  2003 以下
                _book = new HSSFWorkbook(fileStream);
            else
                _book = new XSSFWorkbook(fileStream);
        }



        /// <summary>
        /// sheet 名称，如果为空取第一个sheet
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public ExcelRead GetSheet(string sheetName = "")
        {
            if (sheetName != null)
            {
                _sheet = _book.GetSheet(sheetName);
                if (_sheet == null)
                    _sheet = _book.GetSheetAt(0);
            }
            else
            {
                _sheet = _book.GetSheetAt(0);
            }
            if (_sheet == null)
                throw new NullReferenceException("Excel中无Sheet");

            return this;
        }

        /// <summary>
        /// Sheet转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isFirstColumnNameAndSkip">当第一行为列名，而且要跳过此行，设为True否则为False</param>
        /// <param name="mapRow">IRow：为每行的数据。int:每行在sheet中的下标，从0开始。 遍历每行，如果不需要此行数据返回null。</param>
        /// <returns></returns>
        public List<T> ToList<T>(Func<SheetRow, int, T> mapRow, bool isFirstColumnNameAndSkip = true)
        {
            if (_sheet == null)
            {
                throw new NullReferenceException("先调用GetSheet方法");
            }

            List<T> list = new List<T>();
            IRow firstRow = _sheet.GetRow(0);
            int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

            int startRow = _sheet.FirstRowNum; //首行 的下标
            int rowCount = _sheet.LastRowNum;

            //如果第一行是列名
            if (isFirstColumnNameAndSkip)
            {
                startRow = startRow + 1;

                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        firstRow.GetCell(i).SetCellType(CellType.String);
                        string cellValue = cell.StringCellValue.TrimNull();
                        //保存名称对应的下标
                        if (!string.IsNullOrWhiteSpace(cellValue))
                            _dicColumnName.Add(cellValue, i);
                    }
                }
            }

            for (int i = startRow; i <= rowCount; ++i)
            {
                IRow row = _sheet.GetRow(i);
                if (row == null) continue; //没有数据的行默认是null　
                T item = mapRow(new SheetRow(row, _dicColumnName), i);
                if (item != null)
                    list.Add(item);

            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapRow">IRow：为每行的数据。int:每行下标，从0开始。 遍历每行，如果不需要此行数据返回null。</param>
        /// <param name="sheetName">sheet 名称，如果为空取第一个sheet</param>
        /// <param name="isFirstColumnNameAndSkip">当第一行为列名，而且要跳过此行，设为True否则为False</param>
        /// <returns></returns>
        public List<T> SheetToList<T>(Func<SheetRow, int, T> mapRow, string sheetName = "", bool isFirstColumnNameAndSkip = true)
        {
            GetSheet(sheetName);
            return ToList<T>(mapRow, isFirstColumnNameAndSkip);
        }


        public void Dispose()
        {
            _sheet = null;
        }

    }

    public class SheetRow
    {
        private readonly IRow _row;
        private readonly Dictionary<string, int> _dicColumnName;
        public SheetRow(IRow row, Dictionary<string, int> dicColumnName)
        {
            _row = row;
            _dicColumnName = dicColumnName;
        }

        private int GetColumnIndex(string sheetColumnName)
        {
            if (_dicColumnName.Keys.Contains(sheetColumnName))
                return _dicColumnName[sheetColumnName];
            throw new Exception(sheetColumnName + " 列名在sheet中不存在");
        }
        public string GetString(string sheetColumnName)
        {
            return GetString(GetColumnIndex(sheetColumnName));
        }



        public DateTime? GetDateTime(string sheetColumnName, DateTime? defVal = null)
        {
            return GetDateTime(GetColumnIndex(sheetColumnName), defVal);
        }

        public double? GetDouble(string sheetColumnName, double? defVal = null)
        {
            return GetDouble(GetColumnIndex(sheetColumnName), defVal);
        }

        public int? GetInt(string sheetColumnName, int? defVal = null)
        {
            return GetInt(GetColumnIndex(sheetColumnName), defVal);
        }

        public string GetString(int column)
        {
            var cell = _row.GetCell(column);
            if (cell == null)
                return null;

            return cell.ToString().Trim();
        }

        public DateTime? GetDateTime(int column, DateTime? defVal = null)
        {
            var cell = _row.GetCell(column);
            if (cell == null)
                return defVal;

            string cellValue = string.Empty;
            //Cell为非Numeric时，调用IsCellDateFormatted方法会报错，所以先要进行类型判断
            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                cellValue = cell.DateCellValue.ToLongDateString();
            else
            {
                cellValue = cell.StringCellValue.Trim();
            }
            DateTime dt;
            if (DateTime.TryParse(cellValue, out dt))
                return dt;

            return defVal;
        }

        public double? GetDouble(int column, double? defVal = null)
        {
            var cell = _row.GetCell(column);
            if (cell == null)
                return defVal;
            return cell.NumericCellValue;
        }

        public int? GetInt(int column, int? defVal = null)
        {
            var cell = _row.GetCell(column);
            if (cell == null)
                return defVal;
            int value = int.MinValue;
            if (int.TryParse(GetString(column), out value))
                return value;
            return defVal;
        }

        /// <summary>
        /// 可以解析字符串 True 和 数值 1=true
        /// </summary>
        /// <param name="column"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public bool? GetBool(int column, bool? defVal = null)
        {
            var cell = _row.GetCell(column);
            if (cell == null)
                return defVal;

            bool b;
            if (bool.TryParse(GetString(column), out b))
            {
                return b;
            }
            else
            {
                var val = GetInt(column);
                if (val.HasValue)
                    return val.Value == 1;
            }

            return defVal;
        }

        public short GetLastCellNum()
        {
            return _row.LastCellNum;
        }

    }
}