using System.Collections.Generic;

namespace Logic
{
    public class CellRegrowthResult
    {
        public List<BioCell> RegrownCells { get; }
        public Dictionary<int, int> PlayerNumberToNumberOfDeadCellsEliminated { get; }
        public Dictionary<int, int> PlayerNumberToNumberOfRegrownCells { get; }


        public CellRegrowthResult(List<BioCell> regrownCells, Dictionary<int, int> playerNumberToNumberOfDeadCellsEliminated,
            Dictionary<int, int> playerNumberToNumberOfRegrownCells)
        {
            RegrownCells = regrownCells;
            PlayerNumberToNumberOfDeadCellsEliminated = playerNumberToNumberOfDeadCellsEliminated;
            PlayerNumberToNumberOfRegrownCells = playerNumberToNumberOfRegrownCells;
        }
    }
}