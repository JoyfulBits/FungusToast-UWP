using System.Collections.Generic;

namespace Logic
{
    public class NextGenerationResults
    {
        public List<BioCell> NewDeadCells { get; }
        public List<BioCell> NewLiveCells { get; }

        public NextGenerationResults(List<BioCell> newLiveCells, List<BioCell> newDeadCells)
        {
            NewDeadCells = newDeadCells;
            NewLiveCells = newLiveCells;
        }
    }
}