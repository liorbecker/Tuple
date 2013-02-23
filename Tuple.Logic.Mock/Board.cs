using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Board
    {
        private ICard[,] board;

        public Board(ushort maxRow, ushort maxCol)
        {
            board = new Card[maxRow, maxCol];
        }

        public ICard this[IPosition position]
        {
            get
            {
                return this[position.Row, position.Col];
            }

            set
            {
                this[position.Row, position.Col] = value;
            }
        }

        public ICard this[ushort row, ushort col]
        {
            get
            {
                return board[row, col];
            }

            set
            {
                board[row, col] = value;
            }
        }
    }
}
