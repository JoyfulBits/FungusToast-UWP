using System.Collections.Generic;

namespace Logic
{
    public class NextGenerationResults
    {
        public List<BioCell> NewDeadCells { get; }
        public List<BioCell> RegrownCells { get; }
        public List<BioCell> NewLiveCells { get; }

        public NextGenerationResults(List<BioCell> newLiveCells,
            List<BioCell> newDeadCells, List<BioCell> regrownCells)
        {
            NewDeadCells = newDeadCells;
            RegrownCells = regrownCells;
            NewLiveCells = newLiveCells;
        }
    }
}