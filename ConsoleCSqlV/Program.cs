using System;
using CSqlV;

namespace ConsoleCSqlV
{
    static class Program
    {
        static void Main(string[] args)
        {
            CsvReader csvReader = new CsvReader(@"N:\Programowanie\C#\Moje projekty\Console\CSqlV\students.csv")
            {
                Separator = ",",
                HasHeader = true
            };

            SqlTableMaker sqlTableMaker = new SqlTableMaker();

            var result = sqlTableMaker.CreateInsertToQuery(csvReader.GetRows());

            foreach(var row in result)
            {
                Console.WriteLine(row);
            }
        }
    }
}
