using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuple.Logic.Interfaces
{
    public class Position
    {
        public ushort Row { get; set; }
        public ushort Col { get; set; }

        public Position(ushort row, ushort col)
        {
            Row = row;
            Col = col;
        }
    }
}
