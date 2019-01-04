using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace Logic
{
    public class BioCell : GridCell
    {
        public IPlayer Player { get; }

        public Color CellColor { get; }
        /// <summary>
        /// Represents the previous player who had this cell alive. This only gets changed upon cell death or takeover.
        /// </summary>
        public IPlayer PreviousPlayer { get; set; } = null;

        //--allowing for good old fashioned property injection
        private readonly ISurroundingCellCalculator _surroundingCellCalculator;

        public BioCell(IPlayer player, int cellIndex, Color cellColor, ISurroundingCellCalculator surroundingCellCalculator) : base(cellIndex, false, false, true, false)
        {
            Player = player;
            CellColor = cellColor;
            OutOfGrid = false;
            OrganicCell = true;
            _surroundingCellCalculator = surroundingCellCalculator;
        }

        public CellGrowthResult RunCellGrowth(Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            SurroundingCells surroundingCells = _surroundingCellCalculator.GetSurroundingCells(this, currentLiveCells, currentDeadCells);

            return Player.CalculateCellGrowth(this, surroundingCells);
        }

    }
}