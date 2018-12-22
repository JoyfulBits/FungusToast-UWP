using System.Collections.Generic;
using Windows.UI;

namespace Logic
{
    public class GridCell
    {
        public bool OutOfGrid { get; set; }
        public bool Empty { get; set; }
        public bool LiveCell { get; set; }

        public static readonly GridCell OutOfGridCell = new GridCell
        {
            OutOfGrid = true,
            Empty = false,
            LiveCell = false
        };

        public static readonly GridCell EmptyCell = new GridCell
        {
            OutOfGrid = false,
            Empty = true
        };
    }
    public class BioCell : GridCell
    {
        public IPlayer Player { get; }
        public int CellIndex { get; }
        public Color CellColor { get; }
        //--allowing for good old fashioned property injection
        public ISurroundingCellCalculator SurroundingCellCalculator { get; set; }

        public BioCell(IPlayer player, int cellIndex, Color cellColor)
        {
            Player = player;
            CellIndex = cellIndex;
            CellColor = cellColor;
            OutOfGrid = false;
            LiveCell = true;
            SurroundingCellCalculator = new SurroundingCellCalculator();
        }

        public List<BioCell> RunCellGrowth(Dictionary<int, BioCell> currentLiveCells)
        {
            var surroundingCells = SurroundingCellCalculator.GetSurroundingCells(this, currentLiveCells);

            return Player.CalculateCellGrowth(this, surroundingCells);
        }

    }
}