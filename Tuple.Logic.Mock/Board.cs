using System.Runtime.Serialization;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    [DataContract]
    public class Board
    {
        [DataMember]
        private ICard[][] board;

        public Board(ushort maxRow, ushort maxCol)
        {
            board = new Card[maxRow][];

            for (int i = 0; i < maxRow; i++)
            {
                board[i] = new Card[maxCol];
            }
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
                return board[row][col];
            }

            set
            {
                board[row][col] = value;
            }
        }
    }
}
