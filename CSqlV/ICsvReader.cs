using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSqlV
{
    public interface ICsvReader
    {
        bool HasHeader { get; set; }
        bool Trim { get; set; }
        bool FillEmpty { get; set; }

        string Separator { get; set; }
    }
}
