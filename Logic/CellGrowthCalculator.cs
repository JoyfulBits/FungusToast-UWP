using System;
using System.Collections.Generic;

namespace Logic
{
    public class CellGrowthCalculator : ICellGrowthCalculator
    {
        private readonly Random _random = new Random();

        public List<BioCell> CalculateCellGrowth(BioCell cell, Player player, SurroundingCells surroundingCells)
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

            return newCells;
        }
    }
}