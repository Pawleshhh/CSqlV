using System;

namespace CSqlV
{
    static class Program
    {
        static void Main(string[] args)
        {
            Csqlv csqlv = new Csqlv();

            csqlv.CreateSqlTable(args[1]);
        }
    }
}
