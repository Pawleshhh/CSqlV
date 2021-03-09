using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    public interface ISqlTableMaker
    {
        bool MultipleInsert { get; set; }
        bool EmptyStringToNULL { get; set; }

        string TableName { get; set; }
    }
}
