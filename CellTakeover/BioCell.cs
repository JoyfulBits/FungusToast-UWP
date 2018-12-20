using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CellTakeover
{
    internal class GridCell
    {
        public bool OutOfGridCell { get; set; }


    }
    internal class BioCell : GridCell
    {
        public int PlayerNumber { get; }
        public int CellIndex { get; }
        public Color CellColor { get; }

        public BioCell(int playerNumber, int cellIndex, Color cellColor)
        {
            PlayerNumber = playerNumber;
            CellIndex = cellIndex;
            CellColor = cellColor;
            OutOfGridCell = false;
        }

        public SurroundingCells GetSurroundingCells(Dictionary<int, BioCell> currentLiveCells)
        {
            return new SurroundingCells();
        }
    }
}