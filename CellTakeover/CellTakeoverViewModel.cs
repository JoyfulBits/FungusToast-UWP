using System.Collections.Generic;
using Logic;

namespace CellTakeover
{
    public class CellTakeoverViewModel
    {
        public Dictionary<int, BioCell> CurrentLiveCells { get; set; } = new Dictionary<int, BioCell>();
        public List<Player> Players { get; set; } = new List<Player>();
    }
}