using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tuple.Infra.Log;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    [DataContract]
    public class Deck : IDeck
    {
        [DataMember]
        private Stack<ICard> cards;

        public Deck()
        {

            HashSet<ICard> cardsSet = new HashSet<ICard>();
            foreach (var symbol in (Symbol[])Enum.GetValues(typeof(Symbol)))
            {
                foreach (var color in (Color[])Enum.GetValues(typeof(Color)))
                {
                    foreach (var number in (Number[])Enum.GetValues(typeof(Number)))
                    {
                        foreach (var shading in (Shading[])Enum.GetValues(typeof(Shading)))
                        {
                            cardsSet.Add(new Card(symbol, color, number, shading));
                        }
                    }
                }
            }
            var shuffeledCards = cardsSet.OrderBy(c => Guid.NewGuid());

            cards = new Stack<ICard>(81);
            foreach (var item in shuffeledCards)
            {
                cards.Push(item);
            }
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
