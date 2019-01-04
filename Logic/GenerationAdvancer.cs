using System.Collections.Generic;

namespace Logic
{
    public class GenerationAdvancer
    {
        private readonly ICellRegrowthCalculator _cellRegrowthCalculator;

        public GenerationAdvancer(ICellRegrowthCalculator cellRegrowthCalculator)
        {
            _cellRegrowthCalculator = cellRegrowthCalculator;
        }

        public NextGenerationResults NextGeneration(Dictionary<int, BioCell> currentLiveCells,
            Dictionary<int, BioCell> currentDeadCells, List<IPlayer> players)
        {
            var allNewCellsForGeneration = new List<BioCell>();
            //--create a copy so that the live cells are snapshot and not changing mid-generation
            var copyOfCurrentLiveCells = new Dictionary<int, BioCell>(currentLiveCells);
            var newDeadCells = new List<BioCell>();

            var playerGrowthSummaries = InitializePlayerGrowthSummaries(players);

            foreach (var liveCell in copyOfCurrentLiveCells)
            {
                var cellGrowthResult = liveCell.Value.RunCellGrowth(copyOfCurrentLiveCells, currentDeadCells);

                allNewCellsForGeneration.AddRange(cellGrowthResult.NewLiveCells);

                newDeadCells.AddRange(cellGrowthResult.NewDeadCells);
                var playerGrowthSummary = playerGrowthSummaries[liveCell.Value.Player.PlayerNumber];
                playerGrowthSummary.NewLiveCellCount += cellGrowthResult.NewLiveCells.Count;
                playerGrowthSummary.NewDeadCellCount += cellGrowthResult.NewDeadCells.Count;
            }

            var cellRegrowthResult = _cellRegrowthCalculator.CalculateCellRegrowth(currentDeadCells, currentLiveCells, players);

            return new NextGenerationResults(
                allNewCellsForGeneration, 
                newDeadCells, 
                cellRegrowthResult.RegrownCells, playerGrowthSummaries, 
                cellRegrowthResult.PlayerNumberToNumberOfDeadCellsEliminated, 
                cellRegrowthResult.PlayerNumberToNumberOfRegrownCells);
        }

        private static Dictionary<int, PlayerGrowthSummary> InitializePlayerGrowthSummaries(List<IPlayer> players)
        {
            var playerGrowthSummaries = new Dictionary<int, PlayerGrowthSummary>();
            foreach (var player in players)
            {
                playerGrowthSummaries[player.PlayerNumber] = new PlayerGrowthSummary();
            }

            return playerGrowthSummaries;
        }
    }
}
