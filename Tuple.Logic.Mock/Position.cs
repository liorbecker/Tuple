﻿using System;
using Tuple.Logic.Interfaces;

namespace Tuple.Logic.Mock
{
    public class Position : IPosition
    {
        public ushort Row { get; set; }
        public ushort Col { get; set; }

        public Position(ushort row, ushort col)
        {
            Row = row;
            Col = col;
        }

        public Position(int decimalPosition)
            : this((ushort)(decimalPosition % 3), (ushort)(decimalPosition / 3))
        { }

        public override string ToString()
        {
            return String.Format("[{0},{1}]", Row, Col); ;
        }
    }
}
