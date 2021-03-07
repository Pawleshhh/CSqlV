using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    public class SqlTableMaker
    {

        #region Constructors

        #endregion

        #region Private fields

        //private Dictionary<int, SqlDataType> sqlColumnTypes = new Dictionary<int, SqlDataType>();
        //private Dictionary<int, string> sqlColumnNames = new Dictionary<int, string>();

        private string tableName = "table_name";

        #endregion

        #region Properties

        public bool MultipleInsert { get; set; } = true;
        public bool EmptyStringToNULL { get; set; } = true;

        public string TableName
        {
            get => tableName;
            set
            {
                if (IsNameValid(value))
                    tableName = value;
                else
                    throw new ArgumentException(null, nameof(value));
            }
        }

        #endregion

        #region Methods

        public string[] CreateInsertToQuery(List<string[]> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException(null, nameof(data));

            string[] valueLists = GetValueLists(data, data.Select(n => n.Length).Max(), null);

            return CreateQuery(valueLists, null);
        }

        public string[] CreateInsertToQuery(List<string[]> data, SqlDataType[] types)
        {
            if (!IsArrayValid(types))
                return CreateInsertToQuery(data);

            string[] valueLists = GetValueLists(data, data.Select(n => n.Length).Max(), types);

            return CreateQuery(valueLists, null);
        }

        public string[] CreateInsertToQuery(List<string[]> data, string[] columnNames)
        {
            if (!IsArrayValid(columnNames))
                return CreateInsertToQuery(data);

            string columnList = GetColumnList(columnNames);
            string[] valueLists = GetValueLists(data, columnNames.Length, null);

            return CreateQuery(valueLists, columnList);
        }

        public string[] CreateInsertToQuery(List<string[]> data, string[] columnNames, SqlDataType[] types)
        {
            if (!IsArrayValid(columnNames) && !IsArrayValid(types))
                return CreateInsertToQuery(data);
            if (!IsArrayValid(columnNames))
                return CreateInsertToQuery(data, columnNames);
            if (!IsArrayValid(types))
                return CreateInsertToQuery(data, types);

            string columnList = GetColumnList(columnNames);
            string[] valueLists = GetValueLists(data, columnNames.Length, types);

            return CreateQuery(valueLists, columnList);
        }

        private string[] CreateQuery(string[] valueLists, string columnList)
        {
            //int length = valueLists.Length;
            //string[] queries = new string[length];

            int i = 0;

            if(!string.IsNullOrEmpty(columnList))
                return valueLists.Select(n => string.Format(SqlQueryFormats.InsertRowWithColumnsQuery, tableName, columnList, valueLists[i++])).ToArray();
            else
                return valueLists.Select(n => string.Format(SqlQueryFormats.InsertRowQuery, tableName, valueLists[i++])).ToArray();
        }

        private string GetColumnList(string[] columnNames)
        {
            StringBuilder sqlColumns = new StringBuilder();

            int length = columnNames.Length;
            for(int i = 0; i < length; i++)
            {
                string name = columnNames[i].Trim(); //To be sure

                if (!IsNameValid(name))
                    throw new InvalidOperationException($"{name} is not a valid name for a SQL column.");

                sqlColumns.Append(name);
                if (i < length - 1)
                    sqlColumns.Append(",");
            }

            return sqlColumns.ToString();
        }

        private string[] GetValueLists(List<string[]> data, int columnCount, params SqlDataType[] types)
        {
            bool checkTypes = IsArrayValid(types);
            List<string> sqlValues = new List<string>();

            foreach(var row in data)
            {
                StringBuilder sqlValueList = new StringBuilder();

                int rowLength = row.Length;
                if (rowLength != columnCount)
                    throw new InvalidOperationException("Number of values is not equal to the number of columns.");

                for(int i = 0; i < rowLength; i++)
                {
                    SqlDataType sqlDataType = checkTypes && i < types.Length ? types[i] : SqlDataType.Varchar;
                    string value = ConvertToSqlData(row[i], sqlDataType);

                    sqlValueList.Append(value);
                    if (i < rowLength - 1)
                        sqlValueList.Append(",");
                }

                sqlValues.Add(sqlValueList.ToString());
            }

            return sqlValues.ToArray();
        }

        private string ConvertToSqlData(string data, SqlDataType sqlDataType)
        {
            if (sqlDataType == SqlDataType.Varchar)
            {
                if (EmptyStringToNULL && string.IsNullOrEmpty(data))
                    return "NULL";
                else
                    return $"'{data}'";
            }
            if (sqlDataType == SqlDataType.Int && double.TryParse(data, out _))
                return data;

            throw new ArgumentException(null, nameof(sqlDataType));
        }

        private bool IsNameValid(string name)
            => name != null && !name.Any(char.IsWhiteSpace);

        private bool IsArrayValid(Array array)
            => array != null && array.Length > 0;

        //public string GetColumnName(int columnIndex)
        //{
        //    ThrowColumnIndexOutOfRange(columnIndex);

        //    if (sqlColumnNames.ContainsKey(columnIndex))
        //        return sqlColumnNames[columnIndex];

        //    return null;
        //}

        //public void SetColumnName(int columnIndex, string columnName)
        //{
        //    ThrowColumnIndexOutOfRange(columnIndex);

        //    if(columnName == null)
        //        throw new ArgumentNullException(nameof(columnName));

        //    if (sqlColumnNames.ContainsKey(columnIndex))
        //        sqlColumnNames[columnIndex] = columnName;
        //    else
        //        sqlColumnNames.Add(columnIndex, columnName);
        //}

        //public void SetColumnName(int[] columnIndexes, string[] columnNames)
        //{
        //    if (columnIndexes == null)
        //        throw new ArgumentNullException(nameof(columnIndexes));
        //    if (columnNames == null)
        //        throw new ArgumentNullException(nameof(columnNames));
        //    if (columnIndexes.Length != columnNames.Length)
        //        throw new InvalidOperationException("The number of column indexes is not equal to the number of column names.");

        //    int length = columnIndexes.Length;
        //    for (int i = 0; i < length; i++)
        //        SetColumnName(columnIndexes[i], columnNames[i]);
        //}

        //public SqlDataType GetColumnType(int columnIndex)
        //{
        //    ThrowColumnIndexOutOfRange(columnIndex);

        //    if (sqlColumnTypes.ContainsKey(columnIndex))
        //        return sqlColumnTypes[columnIndex];

        //    return SqlDataType.Varchar;
        //}

        //public void SetColumnType(int columnIndex, SqlDataType sqlDataType)
        //{
        //    ThrowColumnIndexOutOfRange(columnIndex);

        //    if (!Enum.IsDefined(typeof(SqlDataType), sqlDataType))
        //        throw new ArgumentException(null, nameof(sqlDataType));

        //    if (sqlColumnTypes.ContainsKey(columnIndex))
        //        sqlColumnTypes[columnIndex] = sqlDataType;
        //    else
        //        sqlColumnTypes.Add(columnIndex, sqlDataType);
        //}

        //public void SetColumnType(int[] columnIndexes, SqlDataType[] sqlDataTypes)
        //{
        //    if (columnIndexes == null)
        //        throw new ArgumentNullException(nameof(columnIndexes));
        //    if (sqlDataTypes == null)
        //        throw new ArgumentNullException(nameof(sqlDataTypes));
        //    if (columnIndexes.Length != sqlDataTypes.Length)
        //        throw new InvalidOperationException("The number of column indexes is not equal to the number of SQL data types.");

        //    int length = columnIndexes.Length;
        //    for (int i = 0; i < length; i++)
        //        SetColumnType(columnIndexes[i], sqlDataTypes[i]);
        //}

        //private void ThrowColumnIndexOutOfRange(int columnIndex)
        //{
        //    if(columnIndex <= 0)
        //        throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"The {nameof(columnIndex)} argument is out of range.");
        //}

        #endregion

    }

    public enum SqlDataType
    {
        Varchar,
        Int
    }
}
