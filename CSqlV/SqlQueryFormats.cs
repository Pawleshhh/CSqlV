using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    internal static class SqlQueryFormats
    {

        public static string InsertRowQuery { get; } = "INSERT INTO {0} VALUES ({1});";
        public static string InsertRowWithColumnsQuery { get; } = "INSERT INTO {0} ({1}) VALUES ({2});";

        //public static string CreateTable { get; } = "CREATE TABLE {0} ({1});";

    }
}
