﻿using System;
using System.Collections.Generic;
using Tuple.Infra.Log;
using Tuple.Logic.Common;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Game : IGame
    {
        private const ushort rows = 3;
        public const ushort cols = 6;

        private IDeck deck;
        private Board board;

        private GameStats gameStats;

        private readonly Object boardLocker = new Object();

        public Game()
        {
            gameStats = new GameStats();

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
                    UpdateGameStats(
                    board[firstCard.Position],
                    board[secondCard.Position],
                    board[thirdCard.Position]);

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
                return !deck.IsEmpty() && (CountAllOpenedCards() < 12 || !Util.isThereSet(GetAllOpenedCards()) || (CountAllOpenedCards() % 3 != 0));
            }
        }

        public ICardWithPosition OpenCard()
        {
            lock (boardLocker)
            {
                Position position = FindOpenSpace();
                ICard newCard = deck.GetNextCard();

                board[position] = newCard;

                if (Util.isThereSet(GetAllOpenedCards()))
                {
                    var set = Util.FindLegalSet(GetAllOpenedCards());
                    MetroEventSource.Log.Debug(set.Item1 + " - " + set.Item2 + " - " + set.Item3);
                }
                else
                {
                    MetroEventSource.Log.Debug("GAME: No set on board.");
                }

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
            for (ushort j = 0; j < cols; j++)
            {
                for (ushort i = 0; i < rows; i++)
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

        public GameStats GetGameStats()
        {
            return gameStats;
        }

        private void UpdateGameStats(ICard firstCard, ICard secondCard, ICard thirdCard)
        {
            bool sameColor = (firstCard.Color == secondCard.Color && firstCard.Color == thirdCard.Color);
            bool sameSymbol = (firstCard.Symbol == secondCard.Symbol && firstCard.Symbol == thirdCard.Symbol);
            bool sameNumber = (firstCard.Number == secondCard.Number && firstCard.Number == thirdCard.Number);
            bool sameShading = (firstCard.Shading == secondCard.Shading && firstCard.Shading == thirdCard.Shading);

            if (sameColor)
            {
                gameStats.SameColor++;
            }
            if (sameSymbol)
            {
                gameStats.SameSymbol++;
            }
            if (sameNumber)
            {
                gameStats.SameNumber++;
            }
            if (sameShading)
            {
                gameStats.SameShading++;
            }

            if (!sameColor && !sameNumber && !sameShading && !sameSymbol)
            {
                gameStats.Different++;
            }
        }
    }
}
