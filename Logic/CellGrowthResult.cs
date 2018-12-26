using System.Collections.Generic;

namespace Logic
{
    public class CellGrowthResult
    {
        public CellGrowthResult(List<BioCell> newLiveCells, List<BioCell> newDeadCells)
        {
            NewLiveCells = newLiveCells;
            NewDeadCells = newDeadCells;
        }

        public List<BioCell> NewLiveCells { get; }
        public List<BioCell> NewDeadCells { get; }
    }
}