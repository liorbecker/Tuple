using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Position : IPosition
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
