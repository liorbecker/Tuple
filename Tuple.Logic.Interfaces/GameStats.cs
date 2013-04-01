using System;
using System.Runtime.Serialization;

namespace Tuple.Logic.Interfaces
{
    [DataContract]
    public class GameStats
    {
        [DataMember]
        public TimeSpan Time { get; set; }
        [DataMember]
        public bool GameOver { get; set; }
        [DataMember]
        public int Sets{ get; set; }
        [DataMember]
        public int SameColor { get; set; }
        [DataMember]
        public int SameSymbol { get; set; }
        [DataMember]
        public int SameShading { get; set; }
        [DataMember]
        public int SameNumber { get; set; }
        [DataMember]
        public int Different { get; set; }
    }
}
