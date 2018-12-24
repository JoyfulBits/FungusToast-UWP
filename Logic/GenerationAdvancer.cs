using System.Collections.Generic;

namespace Logic
{
    public class GenerationAdvancer
    {
        public NextGenerationResults NextGeneration(Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var allNewCellsForGeneration = new List<BioCell>();
            //--create a copy so that the live cells are snapshot and not changing mid-generation
            var copyOfCurrentLiveCells = new Dictionary<int, BioCell>(currentLiveCells);
            var newDeadCells = new List<BioCell>();
            foreach (var liveCell in copyOfCurrentLiveCells)
            {
                var cellGrowthResult = liveCell.Value.RunCellGrowth(copyOfCurrentLiveCells, currentDeadCells);

                allNewCellsForGeneration.AddRange(cellGrowthResult.NewLiveCells);

                newDeadCells.AddRange(cellGrowthResult.NewDeadCells);
            }

            return new NextGenerationResults(allNewCellsForGeneration, newDeadCells);
        }
    }
}
