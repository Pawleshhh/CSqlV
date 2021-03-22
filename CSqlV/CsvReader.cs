using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSqlV
{
    internal class CsvReader : ICsvReader
    {

        #region Properties

        public int Start { get; set; }
        public int Count { get; set; }

        public bool HasHeader { get; set; }
        public bool Trim { get; set; }
        public bool FillEmpty { get; set; }

        public string Separator { get; set; } = ",";

        #endregion

        #region Methods

        public List<string[]> GetRows(string csvFile)
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

            if (Count == 0 && Start == 0)
                return line.Split(Separator, splitOptions);
            else
            {
                var row = line.Split(Separator, splitOptions);

                if (Count == 0)
                    return row[Start..^1];
                else
                    return row[Start..(Start + Count)];
            }
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
