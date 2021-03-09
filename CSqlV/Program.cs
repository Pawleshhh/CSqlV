using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;

namespace CSqlV
{
    static class Program
    {

        private static Csqlv csqlv = new Csqlv();

        private static readonly Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Need a path to a csv file.");
                return;
            }

            var p = new OptionSet();
            p.Add("h", _ => csqlv.CsvReader.HasHeader = true);
            p.Add("r", _ => csqlv.CsvReader.Trim = true);
            p.Add("f", _ => csqlv.CsvReader.FillEmpty = true);
            p.Add("s|separator=", s => csqlv.CsvReader.Separator = s);
            p.Add("t|table-name=", t => csqlv.SqlTableMaker.TableName = t);
            p.Add("o|output=", o => csqlv.Output = o);
            //p.Add("c|column-names=", c =>
            //{
            //    string[] names = c.Split(',');
            //    csqlv.SetColumnName()
            //});

            var arguments = p.Parse(args);

            //Console.WriteLine(csqlv.CsvReader.FillEmpty.ToString());
            csqlv.CreateSqlTable(args[0]);
        }

    }
}
