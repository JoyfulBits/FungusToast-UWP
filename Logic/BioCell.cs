using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace Logic
{
    public class BioCell : GridCell
    {
        public IPlayer Player { get; }

        public Color CellColor { get; }
        //--allowing for good old fashioned property injection
        public ISurroundingCellCalculator SurroundingCellCalculator { get; set; }

        public BioCell(IPlayer player, int cellIndex, Color cellColor) : base(cellIndex, false, false, true)
        {
            Player = player;
            CellColor = cellColor;
            OutOfGrid = false;
            LiveCell = true;
            SurroundingCellCalculator = new SurroundingCellCalculator();
        }

        public List<BioCell> RunCellGrowth(Dictionary<int, BioCell> currentLiveCells)
        {
            SurroundingCells surroundingCells = SurroundingCellCalculator.GetSurroundingCells(this, currentLiveCells);

            return Player.CalculateCellGrowth(this, surroundingCells);
        }

    }
}