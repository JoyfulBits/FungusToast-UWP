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
        public GameState PreviousGameState { get; set; }
        public List<GrowthCycle> GrowthCycles { get; set; }
        public int GenerationNumber { get; set; }
        public int RoundNumber { get; set; }
        public int TotalDeadCells { get; set; }
        public int TotalEmptyCells { get; set; }
        public int TotalLiveCells { get; set; }
        public int TotalRegeneratedCells { get; set; }
    }
}
