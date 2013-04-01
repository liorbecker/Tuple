using System.Runtime.Serialization;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    [DataContract]
    public class CardWithPosition : ICardWithPosition
    {
        [DataMember]
        public ICard Card { get; set; }

        [DataMember]
        public IPosition Position { get; set; }

        public CardWithPosition(ICard card, IPosition position)
        {
            Card = card;
            Position = position;
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", Card.ToString(), Position.ToString());
        }
    }
}
