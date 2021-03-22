using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;

namespace CSqlV
{
    static class Program
    {

        private static Csqlv csqlv = new Csqlv();

        static void Main(string[] args)
        {
            try
            {
                //if (args.Length == 0 || args[0] != "--help")
                //{
                //    Console.WriteLine($"Need a path to a csv file.");
                //    return;
                //}

                var p = new OptionSet();
                p.Add("header", _ => csqlv.CsvReader.HasHeader = true);
                p.Add("trim", _ => csqlv.CsvReader.Trim = true);
                p.Add("fill", _ => csqlv.CsvReader.FillEmpty = true);
                p.Add("s|separator=", s => csqlv.CsvReader.Separator = s);
                p.Add("t|table-name=", t => csqlv.SqlTableMaker.TableName = t);
                p.Add("o|output=", o => csqlv.Output = o);
                p.Add<int>("start=", s => csqlv.CsvReader.Start = s);
                p.Add<int>("count=", c => csqlv.CsvReader.Count = c);
                p.Add("types=", types =>
                {
                    var s_types = types.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    foreach(var type in s_types)
                    {
                        csqlv.SetColumnType(GetSqlDataType(type));
                    }
                });
                p.Add("columns=", columns =>
                {
                    var s_columns = columns.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    foreach (var column in s_columns)
                    {
                        csqlv.SetColumnName(column);
                    }
                });
                p.Add("help", _ => PrintHelp());

                var arguments = p.Parse(args);

                //Console.WriteLine(csqlv.CsvReader.FillEmpty.ToString());
                csqlv.CreateSqlTable(args[0]);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        static SqlDataType GetSqlDataType(string type)
        {
            if (type == "i")
                return SqlDataType.Int;
            else
                return SqlDataType.Varchar;
        }

        static void PrintHelp()
        {
            Console.WriteLine("--header  - CSV has header.");
            Console.WriteLine("--trim  - Trim string data.");
            Console.WriteLine("--fill  - Fill empty data.");
            Console.WriteLine("-s: - CSV separator.");
            Console.WriteLine("-t: - Name of the SQL table.");
            Console.WriteLine("-o: - Path to the output file with sql queries.");
        }

    }
}
