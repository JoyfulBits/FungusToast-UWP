using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class CellRegrowthCalculator : ICellRegrowthCalculator
    {
        private readonly ISurroundingCellCalculator _surroundingCellCalculator;
        private Random _random = new Random();

        public CellRegrowthCalculator(ISurroundingCellCalculator surroundingCellCalculator)
        {
            _surroundingCellCalculator = surroundingCellCalculator;
        }

        public List<BioCell> CalculateCellRegrowth(Dictionary<int, BioCell> currentDeadCells, Dictionary<int, BioCell> currentLiveCells)
        {
            var regrownCells = new List<BioCell>();

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
                    regrownCells.Add(playerWhoRegeneratesCell.RegrowCell(deadCell));
                }
            }

            return regrownCells;
        }
    }
}