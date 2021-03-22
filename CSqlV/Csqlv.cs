using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    public class Csqlv
    {

        #region Constructors

        #endregion

        #region Private fields

        private List<SqlDataType> sqlColumnTypes = new List<SqlDataType>();
        private List<string> sqlColumnNames = new List<string>();

        private readonly SqlTableMaker sqlTableMaker = new SqlTableMaker();
        private readonly CsvReader csvReader = new CsvReader();

        #endregion

        #region Properties

        public IReadOnlyList<SqlDataType> ColumnTypes => sqlColumnTypes;
        public IReadOnlyList<string> ColumnNames => sqlColumnNames;

        public ICsvReader CsvReader => csvReader;
        public ISqlTableMaker SqlTableMaker => sqlTableMaker;

        public string Output { get; set; } = "./table.db";

        #endregion

        #region Methods

        public string[] CreateSqlTable(string csvFile)
        {
            using(StreamWriter writer = new StreamWriter(Output))
            {
                var rows = csvReader.GetRows(csvFile);
                string[] queries = sqlTableMaker.CreateInsertToQuery(rows, sqlColumnTypes.ToArray());

                foreach (var query in queries)
                {
                    writer.WriteLine(query);
                    Console.WriteLine(query);
                }

                return queries;
            }
        }

        public string GetColumnName(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            return sqlColumnNames[columnIndex];
        }

        public void SetColumnName(string columnName)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            sqlColumnNames.Add(columnName);
        }

        public void SetColumnName(string[] columnNames)
        {
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));

            int length = columnNames.Length;
            for (int i = 0; i < length; i++)
                SetColumnName(columnNames[i]);
        }

        public void RemoveColumnName(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            sqlColumnNames.RemoveAt(columnIndex);
        }

        public void ClearColumnNames()
            => sqlColumnNames.Clear();

        public SqlDataType GetColumnType(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            if (columnIndex < sqlColumnTypes.Count)
                return sqlColumnTypes[columnIndex];

            return SqlDataType.Varchar;
        }

        public void SetColumnType(SqlDataType sqlDataType)
        {
            if (!Enum.IsDefined(typeof(SqlDataType), sqlDataType))
                throw new ArgumentException(null, nameof(sqlDataType));

            sqlColumnTypes.Add(sqlDataType);
        }

        public void SetColumnType(SqlDataType[] sqlDataTypes)
        {
            if (sqlDataTypes == null)
                throw new ArgumentNullException(nameof(sqlDataTypes));
            int length = sqlDataTypes.Length;
            for (int i = 0; i < length; i++)
                SetColumnType(sqlDataTypes[i]);
        }

        public void RemoveColumnType(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            sqlColumnTypes.RemoveAt(columnIndex);
        }

        public void ClearColumnTypes()
            => sqlColumnTypes.Clear();

        private void ThrowColumnIndexOutOfRange(int columnIndex)
        {
            if (columnIndex <= 0)
                throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"The {nameof(columnIndex)} argument is out of range.");
        }

        #endregion

    }
}
