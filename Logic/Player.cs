using System.Collections.Generic;
using Windows.UI;

namespace Logic
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int PlayerNumber { get; set; }


        //--allow for old-fashioned property injection
        public ICellGrowthCalculator CellGrowthCalculator { get; set; }

        public Player(string name, Color playerCellColor, int playerNumber)
        {
            Name = name;
            Color = playerCellColor;
            PlayerNumber = playerNumber;

            CellGrowthCalculator = new CellGrowthCalculator();
        }

        public BioCell MakeCell(int cellIndex)
        {
            return new BioCell(this, cellIndex, Color);
        }

        public List<BioCell> CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells)
        {
            throw new System.NotImplementedException();
        }
    }
}