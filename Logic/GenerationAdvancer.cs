using System.Collections.Generic;

namespace Logic
{
    public class GenerationAdvancer
    {
        public List<BioCell> NextGeneration(Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var allNewCellsForGeneration = new List<BioCell>();
            var copyOfCurrentLiveCells = new Dictionary<int, BioCell>(currentLiveCells);
            foreach (var liveCell in copyOfCurrentLiveCells)
            {
                var cellGrowthResult = liveCell.Value.RunCellGrowth(copyOfCurrentLiveCells);
                foreach (var newCell in cellGrowthResult.NewLiveCells)
                {
                    allNewCellsForGeneration.Add(newCell);
                }
            }

            return allNewCellsForGeneration;
        }
    }
}
