using System;
using System.Collections.Generic;

namespace Logic
{
    public class CellGrowthCalculator : ICellGrowthCalculator
    {
        private readonly Random _random = new Random();

        public CellGrowthResult CalculateCellGrowth(BioCell cell, IPlayer player, SurroundingCells surroundingCells)
        {
            var emptyCells = surroundingCells.EmptyCells;

            var newCells = new List<BioCell>();
            foreach (var emptyCell in emptyCells)
            {
                var growthChancePercentage = player.GrowthScorecard.GetGrowthChance(emptyCell.RelativePosition);
                if (_random.Next(0, 99) < growthChancePercentage)
                {
                    newCells.Add(player.MakeCell(emptyCell.CellIndex));
                }
            }

            var newDeadCells = new List<BioCell>();
            if (surroundingCells.SurroundedByLiveCells)
            {
                if (_random.Next(0, 99) < player.GrowthScorecard.DeathChanceForStarvedCells)
                {
                    cell.Dead = true;
                    newDeadCells.Add(cell);
                    cell.Player.DeadCells++;
                }
            }

            return new CellGrowthResult(newCells, newDeadCells);
        }
    }
}