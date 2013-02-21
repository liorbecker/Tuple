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
    }
}
