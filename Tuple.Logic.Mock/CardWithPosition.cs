﻿using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    class CardWithPosition : ICardWithPosition
    {
        public ICard Card { get; set; }

        public IPosition Position { get; set; }

        public CardWithPosition(ICard card, IPosition position)
        {
            Card = card;
            Position = position;
        }
    }
}
