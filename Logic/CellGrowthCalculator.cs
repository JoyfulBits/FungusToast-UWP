using System;
using System.Collections.Generic;

namespace Logic
{
    public class CellGrowthCalculator : ICellGrowthCalculator
    {
        public const int MinimumLiveCellsForCellDeath = 5;

        private readonly Random _random = new Random();

        public CellGrowthResult CalculateCellGrowth(BioCell cell, IPlayer player, SurroundingCells surroundingCells)
        {
            var emptyCells = surroundingCells.EmptyCells;

            var newCells = GrowNewCells(player, emptyCells);

            var newDeadCells = CheckForCellDeath(cell, player, surroundingCells);

            return new CellGrowthResult(newCells, newDeadCells);
        }

        private List<BioCell> CheckForCellDeath(BioCell cell, IPlayer player, SurroundingCells surroundingCells)
        {
            var newDeadCells = new List<BioCell>();
            if (player.TotalCells >= MinimumLiveCellsForCellDeath 
             && (CellDiesOfStarvation(surroundingCells.SurroundedByLiveCells, player) || CellDiesRandomly(player)))
            {
                cell.Dead = true;
                newDeadCells.Add(cell);
                cell.Player.DeadCells++;
            }

            return newDeadCells;
        }

        private bool CellDiesRandomly(IPlayer player)
        {
            return _random.Next(0, 99) < player.HealthyCellDeathChancePercentage;
        }

        private bool CellDiesOfStarvation(bool surroundingCellsSurroundedByLiveCells, IPlayer player)
        {
            return surroundingCellsSurroundedByLiveCells &&
                   _random.Next(0, 99) < player.GrowthScorecard.DeathChanceForStarvedCells;
        }

        private List<BioCell> GrowNewCells(IPlayer player, List<GridCell> emptyCells)
        {
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