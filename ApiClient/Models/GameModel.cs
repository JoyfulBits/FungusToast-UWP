using System;
using System.Collections.Generic;

namespace ApiClient.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public int NumberOfHumanPlayers { get; set; }
        public int NumberOfAiPlayers { get; set; }
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public string Status { get; set; }
        
        public int NumberOfCells => NumberOfRows * NumberOfColumns;
        public List<PlayerState> Players { get; set; }
        public GameState PreviousGameModel { get; set; }
        public List<GrowthCycle> GrowthCycles { get; set; }
    }
}
