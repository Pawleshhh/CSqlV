using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    public class CsvToSql
    {

        #region Constructors
        public CsvToSql(string csvFile, string separator = ",")
        {
            if (!File.Exists(csvFile))
                throw new ArgumentException(null, nameof(csvFile));
            if (string.IsNullOrWhiteSpace(separator))
                throw new ArgumentException(null, nameof(separator));

            CsvFile = csvFile;
            Separator = separator;
        }
        #endregion

        #region Private fields

        //private int currentLine;

        #endregion

        #region Properties

        public string CsvFile { get; }
        public string Separator { get; set; }
        public string TableName { get; set; } = "table_name";

        public bool HasHeader { get; set; } = true;

        public List<SqlDataType> SQLColumnTypes { get; } = new List<SqlDataType>();

        #endregion

        #region Methods

        public string CreateSQLTable()
        {
            using(StreamReader reader = new StreamReader(CsvFile))
            {
                string line;
                List<string[]> csvRows = new List<string[]>();

                if (HasHeader)
                    reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                    csvRows.Add(GetCsvRow(line));

                //TODO: Fill empty cells in sqlRows
                //need to know maximum count of columns

                List<string> sqlRows = csvRows.Select(n => CreateSQLInsertQuery(n)).ToList();

                StringBuilder sqlQueries = new StringBuilder();

                foreach (var sqlRow in sqlRows)
                    sqlQueries.Append(sqlRow + "\n");

                return sqlQueries.ToString();
            }
        }

        private string[] GetCsvRow(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
                throw new ArgumentException("The row is empty.");

            return row.Split(Separator);
        }

        private string CreateSQLInsertQuery(string[] row)
        {
            if (row == null || row.Length == 0)
                throw new ArgumentException("There are no data in the row.");

            StringBuilder sqlValues = new StringBuilder();
            int sqlTypesCount = SQLColumnTypes.Count;

            for(int i = 0; i < row.Length; i++)
            {
                SqlDataType sqlDataType = sqlTypesCount > 0 && i < sqlTypesCount ? SQLColumnTypes[i] : SqlDataType.Varchar;

                sqlValues.Append(GetData(row[i], sqlDataType));
                if (i < row.Length - 1)
                    sqlValues.Append(", ");
            }

            return string.Format(SqlQueryFormats.InsertRowQuery, TableName, sqlValues.ToString());
        }

        private string GetData(string data, SqlDataType sqlDataType = SqlDataType.Varchar)
        {
            if (sqlDataType == SqlDataType.Varchar)
                return $"'{data}'";
            else if (sqlDataType == SqlDataType.Int)
            {
                if (double.TryParse(data, out _))
                    return data;
                else
                    throw new FormatException("Given value is not a number");
            }

            throw new ArgumentException("Not recognized sql type");
        }

        //private string[] GetHeaders(string topRow)
        //{
        //    if (string.IsNullOrWhiteSpace(topRow))
        //        throw new ArgumentException("There is no row with headers.");

        //    string[] headers = topRow.Split(Separator);

        //    if (headers.Length == 0)
        //        throw new InvalidOperationException($"There are no columns or specified character ({Separator}) is not a separator.");

        //    return headers;
        //}

        #endregion

    }

    //public enum SqlDataType
    //{
    //    Varchar,
    //    Int
    //}
}
