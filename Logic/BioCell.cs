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
        private ISurroundingCellCalculator _surroundingCellCalculator;

        public BioCell(IPlayer player, int cellIndex, Color cellColor, ISurroundingCellCalculator surroundingCellCalculator) : base(cellIndex, false, false, true)
        {
            Player = player;
            CellColor = cellColor;
            OutOfGrid = false;
            LiveCell = true;
            _surroundingCellCalculator = surroundingCellCalculator;
        }

        public List<BioCell> RunCellGrowth(Dictionary<int, BioCell> currentLiveCells)
        {
            SurroundingCells surroundingCells = _surroundingCellCalculator.GetSurroundingCells(this, currentLiveCells);

            return Player.CalculateCellGrowth(this, surroundingCells);
        }

    }
}