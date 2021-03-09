using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    internal class SqlTableMaker : ISqlTableMaker
    {

        #region Constructors

        #endregion

        #region Private fields

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

                if(data.Length > 2 && data[0] == '"' && data[^1] == '"')
                {
                    StringBuilder dataBuilder = new StringBuilder(data);
                    dataBuilder.Remove(0, 1).Remove(dataBuilder.Length - 1, 1).Insert(0, '"').Append('"');
                    data = dataBuilder.ToString();
                }

                data = data.Replace("\"", "");

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

        #endregion

    }

    public enum SqlDataType
    {
        Varchar,
        Int
    }
}
