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

        private Dictionary<int, SqlDataType> sqlColumnTypes = new Dictionary<int, SqlDataType>();
        private Dictionary<int, string> sqlColumnNames = new Dictionary<int, string>();

        private readonly SqlTableMaker sqlTableMaker = new SqlTableMaker();
        private readonly CsvReader csvReader = new CsvReader();

        #endregion

        #region Properties

        public IReadOnlyDictionary<int, SqlDataType> ColumnTypes => sqlColumnTypes;
        public IReadOnlyDictionary<int, string> ColumnNames => sqlColumnNames;

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
                string[] queries = sqlTableMaker.CreateInsertToQuery(rows);

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

            if (sqlColumnNames.ContainsKey(columnIndex))
                return sqlColumnNames[columnIndex];

            return null;
        }

        public void SetColumnName(int columnIndex, string columnName)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            if (sqlColumnNames.ContainsKey(columnIndex))
                sqlColumnNames[columnIndex] = columnName;
            else
                sqlColumnNames.Add(columnIndex, columnName);
        }

        public void SetColumnName(int[] columnIndexes, string[] columnNames)
        {
            if (columnIndexes == null)
                throw new ArgumentNullException(nameof(columnIndexes));
            if (columnNames == null)
                throw new ArgumentNullException(nameof(columnNames));
            if (columnIndexes.Length != columnNames.Length)
                throw new InvalidOperationException("The number of column indexes is not equal to the number of column names.");

            int length = columnIndexes.Length;
            for (int i = 0; i < length; i++)
                SetColumnName(columnIndexes[i], columnNames[i]);
        }

        public void RemoveColumnName(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            sqlColumnNames.Remove(columnIndex);
        }

        public void ClearColumnNames()
            => sqlColumnNames.Clear();

        public SqlDataType GetColumnType(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            if (sqlColumnTypes.ContainsKey(columnIndex))
                return sqlColumnTypes[columnIndex];

            return SqlDataType.Varchar;
        }

        public void SetColumnType(int columnIndex, SqlDataType sqlDataType)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            if (!Enum.IsDefined(typeof(SqlDataType), sqlDataType))
                throw new ArgumentException(null, nameof(sqlDataType));

            if (sqlColumnTypes.ContainsKey(columnIndex))
                sqlColumnTypes[columnIndex] = sqlDataType;
            else
                sqlColumnTypes.Add(columnIndex, sqlDataType);
        }

        public void SetColumnType(int[] columnIndexes, SqlDataType[] sqlDataTypes)
        {
            if (columnIndexes == null)
                throw new ArgumentNullException(nameof(columnIndexes));
            if (sqlDataTypes == null)
                throw new ArgumentNullException(nameof(sqlDataTypes));
            if (columnIndexes.Length != sqlDataTypes.Length)
                throw new InvalidOperationException("The number of column indexes is not equal to the number of SQL data types.");

            int length = columnIndexes.Length;
            for (int i = 0; i < length; i++)
                SetColumnType(columnIndexes[i], sqlDataTypes[i]);
        }

        public void RemoveColumnType(int columnIndex)
        {
            ThrowColumnIndexOutOfRange(columnIndex);

            sqlColumnTypes.Remove(columnIndex);
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
