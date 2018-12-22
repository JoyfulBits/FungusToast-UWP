using System;
using System.Collections.Generic;
using Windows.UI;

namespace Logic
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int PlayerNumber { get; set; }

        public int BaseCellGrowthPercentageChance { get; set; } = 10;
        public GrowthScorecard GrowthScorecard { get; set; } = new GrowthScorecard();

        private readonly Random _random = new Random();
        private ICellGrowthCalculator _cellGrowthCalculator;

        public Player(string name, Color playerCellColor, int playerNumber, ICellGrowthCalculator cellGrowthCalculator)
        {
            Name = name;
            Color = playerCellColor;
            PlayerNumber = playerNumber;

            _cellGrowthCalculator = cellGrowthCalculator;
        }

        public BioCell MakeCell(int cellIndex)
        {
            return new BioCell(this, cellIndex, Color);
        }

        public List<BioCell> CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells)
        {
            return _cellGrowthCalculator.CalculateCellGrowth(cell, GrowthScorecard, surroundingCells);
            //var newCells = new List<BioCell>();
            //if (surroundingCells.TopLeftCell.Empty)
            //{
            //    if (TryGrowInTopLeftCell(surroundingCells.TopLeftCell.CellIndex, out var newCell))
            //    {
            //        newCells.Add(newCell);
            //    }
            //}

            //return newCells;
        }

        private bool TryGrowInTopLeftCell(int emptyIndex, out BioCell newCell)
        {
            if (_random.Next(0, 100) < BaseCellGrowthPercentageChance)
            {
                newCell = MakeCell(emptyIndex);
                return true;
            }

            newCell = null;
            return false;
        }
    }
}