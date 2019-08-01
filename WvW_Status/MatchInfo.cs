using System.Collections.Generic;
using System.Drawing;

namespace WvW_Status
{
    class MatchInfo
    {                
        public string Name { get; set; }        
        public Color Color { get; set; }
        public string LinksToolTip { get; set; }
        public int VictoryPoints { get; set; }
        public int LowVictoryPoints { get; set; }
        public int HighVictoryPoints { get; set; }        
        public double LongVP { get; set; }
        public string VictoryPointsToolTip { get; set; }
        public int Score { get; set; }
        public bool Locked { get; set; }
    }
}