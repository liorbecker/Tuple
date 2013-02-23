using System;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Card : ICard
    {
        public Card(Symbol symbol, Color color, Number number, Shading shading)
        {
            Symbol = symbol;
            Color = color;
            Number = number;
            Shading = shading;
        }

        public Symbol Symbol
        {
            get;
            private set;
        }

        public Color Color
        {
            get;
            private set;
        }

        public Number Number
        {
            get;
            private set;
        }

        public Shading Shading
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return String.Format("[{0},{1},{2},{3}]", Symbol, Color, Number, Shading);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Card;
            if (other == null)
            {
                return false;
            }
            return this.Equals(other);
        }

        public bool Equals(Card card)
        {
            if ((object)card == null)
            {
                return false;
            }

            return (card.Color == Color) && (card.Number == Number) && (card.Shading == Shading) && (card.Symbol == Symbol);
        }

        public override int GetHashCode()
        {
            return ((int)Color * 1) + ((int)Number * 100) + ((int)Shading * 10000) + ((int)Symbol * 1000000);
        }
    }
}
