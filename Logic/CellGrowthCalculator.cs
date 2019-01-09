using System;
using System.Collections.Generic;

namespace Logic
{
    public class CellGrowthCalculator : ICellGrowthCalculator
    {
        public const int MinimumLiveCellsForCellDeath = 5;

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
            if (player.LiveCells >= MinimumLiveCellsForCellDeath 
             && (CellDiesOfStarvation(surroundingCells.SurroundedByLiveCells, player) || CellDiesRandomly(player)))
            {
                cell.Dead = true;
                cell.PreviousPlayer = cell.Player;
                newDeadCells.Add(cell);
            }

            return newDeadCells;
        }

        private bool CellDiesRandomly(IPlayer player)
        {
            //--since ApoptosisChancePercentage is a double, need to add an order of magnitude for precision
            return RandomNumberGenerator.Random.Next(0, 999) < player.GrowthScorecard.ApoptosisChancePercentage * 10;
        }

        private bool CellDiesOfStarvation(bool surroundingCellsSurroundedByLiveCells, IPlayer player)
        {
            return surroundingCellsSurroundedByLiveCells &&
                   RandomNumberGenerator.Random.Next(0, 99) < player.GrowthScorecard.DeathChanceForStarvedCells;
        }

        private List<BioCell> GrowNewCells(IPlayer player, List<GridCell> emptyCells)
        {
            var newCells = new List<BioCell>();
            foreach (var emptyCell in emptyCells)
            {
                var growthChancePercentage = player.GrowthScorecard.GetGrowthChance(emptyCell.RelativePosition);
                if (RandomNumberGenerator.Random.Next(0, 99) < growthChancePercentage)
                {
                    newCells.Add(player.MakeCell(emptyCell.CellIndex));
                }
            }

            return newCells;
        }
    }
}