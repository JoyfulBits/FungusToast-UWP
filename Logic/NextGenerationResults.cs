using System.Collections.Generic;

namespace Logic
{
    public class NextGenerationResults
    {
        public List<BioCell> NewDeadCells { get; }
        public List<BioCell> RegrownCells { get; }
        public List<BioCell> NewLiveCells { get; }
        public Dictionary<int, PlayerGrowthSummary> PlayerGrowthSummaries { get;}
        public Dictionary<int, int> PlayerNumberToNumberOfDeadCellsEliminated { get; }
        public Dictionary<int, int> PlayerNumberToNumberOfRegrownCells { get; }

        public NextGenerationResults(List<BioCell> newLiveCells,
            List<BioCell> newDeadCells, List<BioCell> regrownCells,
            Dictionary<int, PlayerGrowthSummary> playerGrowthSummaries, 
            Dictionary<int, int> playerNumberToNumberOfDeadCellsEliminated, 
            Dictionary<int, int> playerNumberToNumberOfRegrownCells)
        {
            NewDeadCells = newDeadCells;
            RegrownCells = regrownCells;
            NewLiveCells = newLiveCells;
            PlayerGrowthSummaries = playerGrowthSummaries;
            PlayerNumberToNumberOfDeadCellsEliminated = playerNumberToNumberOfDeadCellsEliminated;
            PlayerNumberToNumberOfRegrownCells = playerNumberToNumberOfRegrownCells;
        }
    }
}