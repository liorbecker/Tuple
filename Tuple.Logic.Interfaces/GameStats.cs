﻿
namespace Tuple.Logic.Interfaces
{
    public class GameStats
    {
        public int TicksInSec { get; set; }

        public int Sets{ get; set; }
        
        public int SameColor { get; set; }
        public int SameSymbol { get; set; }
        public int SameShading { get; set; }
        public int SameNumber { get; set; }
        public int Different { get; set; }
    }
}
