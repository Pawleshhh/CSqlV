using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSqlV
{
    public class CsvReader
    {

        #region Constructors

        public CsvReader(string csvFile)
        {
            if (!File.Exists(csvFile))
                throw new ArgumentException($"There is no such file ({csvFile})", nameof(csvFile));

            this.csvFile = csvFile;
        }

        #endregion

        #region Private fields

        private readonly string csvFile;

        #endregion

        #region Properties

        public bool HasHeader { get; set; } = true;
        public bool Trim { get; set; } = true;
        public bool FillEmpty { get; set; } = true;

        public string Separator { get; set; } = ",";

        #endregion

        #region Methods

        public List<string[]> GetRows()
        {
            using(StreamReader reader = new StreamReader(csvFile))
            {
                //If csv file has a header row then skip it.
                if (HasHeader)
                    reader.ReadLine();

                string line;
                List<string[]> rows = new List<string[]>();

                while((line = reader.ReadLine()) != null)
                {
                    //Get every cell from csv line
                    rows.Add(GetCsvRow(line));
                }

                //Make string arrays as long as the longest array in the list.
                if (FillEmpty)
                    FillEmptyCells(rows);

                return rows;
            }
        }

        private string[] GetCsvRow(string line)
        {
            StringSplitOptions splitOptions = Trim ? StringSplitOptions.TrimEntries : StringSplitOptions.None;

            return line.Split(Separator, splitOptions);
        }

        private void FillEmptyCells(List<string[]> rows)
        {
            int maxLength = rows.Select(n => n.Length).Max();

            for(int i = 0; i < rows.Count; i++)
            {
                if (rows[i].Length == maxLength)
                    continue;

                string[] copy = Enumerable.Repeat(string.Empty, maxLength).ToArray();
                Array.Copy(rows[i], copy, rows[i].Length);
                rows[i] = copy;
            }
        }

        #endregion

    }
}
