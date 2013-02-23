﻿using System;
using System.Collections.Generic;
using Tuple.Infra.Log;
using Tuple.Logic.Common;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Game : IGame
    {
        private const ushort rows = 6;
        public const ushort cols = 3;

        private IDeck deck;
        private Board board;

        private readonly Object boardLocker = new Object();

        public Game()
        {
            deck = new Deck();

            board = new Board(rows, cols);
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

        public bool RemoveSet(ICardWithPosition firstCard, ICardWithPosition secondCard, ICardWithPosition thirdCard)
        {
            lock (boardLocker)
            {
                if (!Util.isLegalSet(
                    board[firstCard.Position],
                    board[secondCard.Position],
                    board[thirdCard.Position]))
                {
                    MetroEventSource.Log.Warn(String.Format("GAME: Trying to remove ilegal set {0}, {1}, {2}",
                        board[firstCard.Position],
                        board[secondCard.Position],
                        board[thirdCard.Position]));

                    return false;
                }
                else
                {
                    board[firstCard.Position] = null;
                    board[secondCard.Position] = null;
                    board[thirdCard.Position] = null;

                    return true;
	            }
            }
        }

        public bool ShouldOpenCard()
        {
            lock (boardLocker)
            {
                return !deck.IsEmpty() && (CountAllOpenedCards() < 12 || !Util.isThereSet(GetAllOpenedCards()));
            }
        }

        public ICardWithPosition OpenCard()
        {
            lock (boardLocker)
            {
                Position position = FindOpenSpace();
                ICard newCard = deck.GetNextCard();

                board[position] = newCard;

                return new CardWithPosition(newCard, position);
            }
        }

        public IEnumerable<ICardWithPosition> GetAllOpenedCards()
        {
            IList<ICardWithPosition> cards = new List<ICardWithPosition>();

            for (ushort i = 0; i < rows; i++)
            {
                for (ushort j = 0; j < cols; j++)
                {
                    if (board[i, j] != null)
                    {
                        cards.Add(new CardWithPosition(board[i, j], new Position(i, j)));
                    }
                }
            }

            return cards;
        }

        public override string ToString()
        {
            String boardString = String.Empty;
            for (ushort i = 0; i < rows; i++)
            {
                for (ushort j = 0; j < cols; j++)
                {
                    if (board[i, j] != null)
                    {
                        boardString += board[i, j].ToString().PadRight(31) + " ";
                    }
                }
                boardString += Environment.NewLine;
            }

            return
                new String('-', 95) + Environment.NewLine +
                "- Deck empty: " + deck.IsEmpty() + " - Has set: " + Util.isThereSet(GetAllOpenedCards()) + Environment.NewLine +
                new String('-', 95) + Environment.NewLine +
                boardString +
                new String('-', 95) + Environment.NewLine;
        }

        private ushort CountAllOpenedCards()
        {
            ushort count = 0;

            for (ushort i = 0; i < rows; i++)
            {
                for (ushort j = 0; j < cols; j++)
                {
                    if (board[i, j] != null)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private Position FindOpenSpace()
        {
            for (ushort i = 0; i < rows; i++)
            {
                for (ushort j = 0; j < cols; j++)
                {
                    if (board[i, j] == null)
                    {
                        return new Position(i, j);
                    }
                }
            }

            MetroEventSource.Log.Critical("GAME: wrong logic, should always find a free space sine the board is large enough (if it's full, there must be a set so this method shouldn't be called.");
            throw new Exception("GAME: wrong logic, should always find a free space sine the board is large enough (if it's full, there must be a set so this method shouldn't be called.");
        }
    }
}
