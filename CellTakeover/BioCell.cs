using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CellTakeover
{
    internal class GridCell
    {
        public bool OutOfGrid { get; set; }

        public static readonly GridCell OutOfGridCell = new GridCell
        {
            OutOfGrid = true
        };
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
            OutOfGrid = false;
        }

        public SurroundingCells GetSurroundingCells(Dictionary<int, BioCell> currentLiveCells)
        {
            var surroundingCells = new SurroundingCells();

            if (OnLeftColumn())
            {
                surroundingCells.TopLeftCell = OutOfGridCell;
                surroundingCells.LeftCell = OutOfGridCell;
                surroundingCells.BottomLeftCell = OutOfGridCell;
            }
            else if (OnRightColumn())
            {
                surroundingCells.TopRightCell = OutOfGridCell;
                surroundingCells.RightCell = OutOfGridCell;
                surroundingCells.BottomRightCell = OutOfGridCell;
            }

            if (OnTopRow())
            {
                surroundingCells.TopLeftCell = OutOfGridCell;
                surroundingCells.TopCell = OutOfGridCell;
                surroundingCells.TopRightCell = OutOfGridCell;
            }else if (OnBottomRow())
            {
                surroundingCells.BottomLeftCell = OutOfGridCell;
                surroundingCells.BottomCell = OutOfGridCell;
                surroundingCells.BottomRightCell = OutOfGridCell;
            }

            return surroundingCells;
        }


        private bool OnTopRow()
        {
            return CellIndex < GameSettings.NumberOfColumnsAndRows;
        }

        private bool OnBottomRow()
        {
            return CellIndex >= GameSettings.NumberOfCells - GameSettings.NumberOfColumnsAndRows;
        }

        private bool OnRightColumn()
        {
            return CellIndex % GameSettings.NumberOfColumnsAndRows == (GameSettings.NumberOfColumnsAndRows - 1);
        }


        private bool OnLeftColumn()
        {
            return CellIndex % GameSettings.NumberOfColumnsAndRows == 0;
        }
    }
}