using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class CellRegrowthCalculator : ICellRegrowthCalculator
    {
        private readonly ISurroundingCellCalculator _surroundingCellCalculator;
        private readonly Random _random = new Random();

        public CellRegrowthCalculator(ISurroundingCellCalculator surroundingCellCalculator)
        {
            _surroundingCellCalculator = surroundingCellCalculator;
        }

        public CellRegrowthResult CalculateCellRegrowth(Dictionary<int, BioCell> currentDeadCells, Dictionary<int, BioCell> currentLiveCells, List<IPlayer> players)
        {
            var regrownCells = new List<BioCell>();
            var playerNumberToNumberOfDeadCellsEliminated = players.ToDictionary(x => x.PlayerNumber, y => 0);
            var playerNumberToNumberOfRegrownCells = players.ToDictionary(x => x.PlayerNumber, y => 0);

            foreach (var deadCell in currentDeadCells.Values)
            {
                var surroundingCells =
                    _surroundingCellCalculator.GetSurroundingCells(deadCell, currentLiveCells, currentDeadCells);

                var surroundingPlayersWithRegrowth = surroundingCells.GetAllSurroundingPlayersWithRegrowth();

                var regrowthContenders = new List<IPlayer>();

                foreach (var player in surroundingPlayersWithRegrowth.Values)
                {
                    if (player.GrowthScorecard.RegrowthChancePercentage > 0 &&
                        _random.Next(0, 99) < player.GrowthScorecard.RegrowthChancePercentage)
                    {
                        regrowthContenders.Add(player);
                    }
                }

                //--the player with the highest Regrowth (or in the case of a tie, the most dead cells) gets to regrow
                var playerWhoRegeneratesCell = regrowthContenders
                    .OrderByDescending(x => x.GrowthScorecard.RegrowthChancePercentage).ThenByDescending(x => x.DeadCells).FirstOrDefault();

                if (playerWhoRegeneratesCell != null)
                {
                    playerNumberToNumberOfDeadCellsEliminated[deadCell.Player.PlayerNumber]++;

                    var regrownCell = playerWhoRegeneratesCell.RegrowCell(deadCell);

                    playerNumberToNumberOfRegrownCells[playerWhoRegeneratesCell.PlayerNumber]++;

                    regrownCells.Add(regrownCell);
                }
            }

            return new CellRegrowthResult(regrownCells, playerNumberToNumberOfDeadCellsEliminated, playerNumberToNumberOfRegrownCells);
        }
    }
}