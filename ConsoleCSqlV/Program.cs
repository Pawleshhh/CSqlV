using System;
using CSqlV;

namespace ConsoleCSqlV
{
    static class Program
    {
        static void Main(string[] args)
        {
            CsvReader csvReader = new CsvReader(@"N:\Programowanie\C#\Moje projekty\Console\CSqlV\ConsoleCSqlV\testData.csv");
            csvReader.HasHeader = true;
            csvReader.Trim = true;

            var result = csvReader.GetRows();

            foreach(var row in result)
            {
                Console.WriteLine(string.Join('|', row));
            }
        }
    }
}
