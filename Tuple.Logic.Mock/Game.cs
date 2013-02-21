using System;
using System.Collections.Generic;
using Tuple.Infra.Log;
using Tuple.Logic.Common;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Game : IGame
    {
        private const int rows = 6;
        public const int cols = 3;

        private IDeck deck;
        private ICard[,] board;

        private const Object boardLocker = new Object();

        public Game()
        {
            deck = new Deck();

            board = new Card[rows, cols];
        }

        public bool IsGameOver()
        {
            lock (boardLocker)
	        {
                if (!deck.IsEmpty() || Util.isThereSet(GetAllOpenedCards()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool RemoveSet(int firstCardRow, int firstCardCol, int secondCardRow, int secondCardCol, int thirdCardRow, int thirdCardCol)
        {
            lock (boardLocker)
            {
                if (!Util.isLegalSet(board[firstCardRow, firstCardCol], board[secondCardRow, secondCardCol], board[thirdCardRow, thirdCardCol]))
                {
                    MetroEventSource.Log.Warn(String.Format("GAME: Trying to remove ilegal set {0}, {1}, {2}",
                        board[firstCardRow, firstCardCol], board[secondCardRow, secondCardCol], board[thirdCardRow, thirdCardCol]));

                    return false;
                }
                else
                {
                    board[firstCardRow, firstCardCol] = null;
                    board[secondCardRow, secondCardCol] = null;
                    board[thirdCardRow, thirdCardCol] = null;

                    return true;
	            }
            }
        }

        public ICard OpenCard(out int row, out int col)
        {
            lock (boardLocker)
            {
                if (deck.IsEmpty() || Util.isThereSet(GetAllOpenedCards()))
                {
                    row = -1;
                    col = -1;

                    return null;
                }
                else
                {
                    FindOpenSpace(out row, out col);
                    return board[row,col];
                }
            }
        }

        private IEnumerable<ICard> GetAllOpenedCards()
        {
            IList<ICard> cards = new List<ICard>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (board[i, j] != null)
                    {
                        cards.Add(board[i, j]);
                    }
                }
            }

            return cards;
        }

        private void FindOpenSpace(out int row, out int col)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (board[i, j] == null)
                    {
                        row = i;
                        col = j;
                        return;
                    }
                }
            }

            MetroEventSource.Log.Critical("GAME: wrong logic, should also find a free space sine the board is large enough (if it's full, there must be a set so this method shouldn't be called.");
            throw new Exception("GAME: wrong logic, should also find a free space sine the board is large enough (if it's full, there must be a set so this method shouldn't be called.");
        }
    }
}
