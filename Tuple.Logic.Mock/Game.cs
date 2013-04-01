using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tuple.Infra.Log;
using Tuple.Logic.Common;
using Tuple.Logic.Interfaces;
using Windows.UI.Xaml;

namespace Tuple.Logic.Mock
{
    [DataContract]
    public class Game : IGame
    {
        # region Consts

        private const ushort rows = 3;
        private const ushort cols = 6;

        # endregion

        # region Private members

        [DataMember]
        private IDeck deck;
        [DataMember]
        private Board board;
        [DataMember]
        private GameStats gameStats;
        [IgnoreDataMember]
        DispatcherTimer timer;

        [DataMember]
        private readonly Object boardLocker = new Object();
        
        # endregion

        # region Public events

        public event EventHandler<Object> SecondPassed;

        # endregion

        # region Constructor

        public Game()
        {
            deck = new Deck();

            board = new Board(rows, cols);

            gameStats = new GameStats()
            {
                Time = new TimeSpan(0)
            };

            
        }

        # endregion

        # region public methods

        public void StartTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += new EventHandler<object>(UpdateSecondPassed);
            }
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public bool IsGameOver()
        {
            lock (boardLocker)
	        {
                return IsGameOverNoLock();
            }
        }

        public bool RemoveSet(IPosition firstCard, IPosition secondCard, IPosition thirdCard)
        {
            lock (boardLocker)
            {
                if (Util.isLegalSet(
                    board[firstCard],
                    board[secondCard],
                    board[thirdCard]))
                {
                    UpdateGameStats(
                    board[firstCard],
                    board[secondCard],
                    board[thirdCard]);

                    board[firstCard] = null;
                    board[secondCard] = null;
                    board[thirdCard] = null;

                    return true;
                }
                else
                {
                    MetroEventSource.Log.Warn(String.Format("GAME: Trying to remove ilegal set {0}, {1}, {2}",
                        board[firstCard],
                        board[secondCard],
                        board[thirdCard]));

                    return false;
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
                    MetroEventSource.Log.Debug("Available set: " + set.Item1 + " - " + set.Item2 + " - " + set.Item3);
                }
                else
                {
                    MetroEventSource.Log.Debug("GAME: No set on board.");
                }

                return new CardWithPosition(newCard, position);
            }
        }

        public GameStats GetGameStats()
        {
            lock (boardLocker)
            {
                return gameStats;
            }
        }

        public IEnumerable<ICardWithPosition> GetAllOpenedCards()
        {
            lock (boardLocker)
            {
                return GetAllOpenedCardsNoLock();
            }
        }

        public override string ToString()
        {
            lock (boardLocker)
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
        }

        # endregion

        # region private methods

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

        private void UpdateGameStats(ICard firstCard, ICard secondCard, ICard thirdCard)
        {
            gameStats.GameOver = IsGameOverNoLock();

            gameStats.Sets++;

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

        private IEnumerable<ICardWithPosition> GetAllOpenedCardsNoLock()
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

        private bool IsGameOverNoLock()
        {
            return deck.IsEmpty() && !Util.isThereSet(GetAllOpenedCardsNoLock());
        }

        private void UpdateSecondPassed(object sender, object data)
        {
            lock (boardLocker)
            {
                gameStats.Time = gameStats.Time.Add(TimeSpan.FromSeconds(1));
            }

            if (SecondPassed != null)
            {
                SecondPassed(this, EventArgs.Empty);
            }
        }

        # endregion
    }
}
