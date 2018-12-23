using System.Collections.Generic;

namespace Logic
{
    public class GenerationAdvancer
    {
        public List<BioCell> NextGeneration(Dictionary<int, BioCell> currentLiveCells)
        {
            var allNewCellsForGeneration = new List<BioCell>();
            var copyOfCurrentLiveCells = new Dictionary<int, BioCell>(currentLiveCells);
            foreach (var liveCell in copyOfCurrentLiveCells)
            {
                var newCells = liveCell.Value.RunCellGrowth(copyOfCurrentLiveCells);
                foreach (var newCell in newCells)
                {
                    allNewCellsForGeneration.Add(newCell);
                }
            }

            return allNewCellsForGeneration;
        }
    }
}
