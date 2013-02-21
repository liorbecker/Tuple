using System;
using System.Collections.Generic;
using System.Linq;
using Tuple.Infra.Log;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Deck : IDeck
    {
        private Stack<ICard> cards;

        public Deck()
        {
            cards = new Stack<ICard>(81);
            
            foreach (var symbol in (Symbol[])Enum.GetValues(typeof(Symbol)))
            {
                foreach (var color in (Color[])Enum.GetValues(typeof(Color)))
                {
                    foreach (var number in (Number[])Enum.GetValues(typeof(Number)))
                    {
                        foreach (var shading in (Shading[])Enum.GetValues(typeof(Shading)))
                        {
                            cards.Push(new Card(symbol, color, number, shading));
                        }
                    }
                }
            }

            cards.OrderBy(c => Guid.NewGuid());
        }

        public ICard GetNextCard()
        {
            if (IsEmpty())
            {
                MetroEventSource.Log.Error("DECK: Trying to get next card while the deck is empty.");
                throw new Exception("DECK: Trying to get next card while the deck is empty.");
            }
            else
            {
                var card = cards.Pop();
                MetroEventSource.Log.Debug(String.Format("DECK: next card is {0}.", card));
                return card;
            }
        }

        public bool IsEmpty()
        {
            return !cards.Any();
        }
    }
}
