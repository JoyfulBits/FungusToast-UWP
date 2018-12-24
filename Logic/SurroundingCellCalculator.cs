using System.Collections.Generic;

namespace Logic
{
    public class SurroundingCellCalculator : ISurroundingCellCalculator
    {
        private readonly int _numberOfRowsAndColumns;

        public SurroundingCellCalculator(int numberOfRowsAndColumns)
        {
            _numberOfRowsAndColumns = numberOfRowsAndColumns;
        }

        public SurroundingCells GetSurroundingCells(
            BioCell bioCell, 
            Dictionary<int, BioCell> currentLiveCells, 
            Dictionary<int, BioCell> currentDeadCells)
        {
            var surroundingCells = new SurroundingCells();
            var checkLeft = true;
            var checkTop = true;
            var checkRight = true;
            var checkBottom = true;
            checkLeft = GetOutOfGridCells(bioCell.CellIndex, surroundingCells, 
                ref checkLeft, ref checkRight, ref checkTop, ref checkBottom);

            GetInGridCells(bioCell.CellIndex, surroundingCells, currentLiveCells, currentDeadCells, checkLeft, checkBottom, checkTop, checkRight);

            return surroundingCells;
        }

        private void GetInGridCells(int cellIndex, SurroundingCells surroundingCells,
            Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells, bool checkLeft,
            bool checkBottom, bool checkTop, bool checkRight)
        {
            if (checkLeft)
            {
                if (checkBottom)
                {
                    surroundingCells.BottomLeftCell = GetBottomLeftCell(cellIndex, currentLiveCells, currentDeadCells);
                }

                surroundingCells.LeftCell = GetLeftCell(cellIndex, currentLiveCells, currentDeadCells);

                if (checkTop)
                {
                    surroundingCells.TopLeftCell = GetTopLeftCell(cellIndex, currentLiveCells, currentDeadCells);
                }
            }

            if (checkTop)
            {
                //--skip top left cell as it's already been set or out of grid

                surroundingCells.TopCell = GetTopCell(cellIndex, currentLiveCells, currentDeadCells);

                if (checkRight)
                {
                    surroundingCells.TopRightCell = GetTopRightCell(cellIndex, currentLiveCells, currentDeadCells);
                }
            }

            if (checkRight)
            {
                //--skip top right cell as it's already been set or out of grid

                surroundingCells.RightCell = GetRightCell(cellIndex, currentLiveCells, currentDeadCells);

                if (checkBottom)
                {
                    surroundingCells.BottomRightCell = GetBottomRightCell(cellIndex, currentLiveCells, currentDeadCells);
                }
            }

            if (checkBottom)
            {
                //--skip bottom right cell as it's already been set or out of grid

                surroundingCells.BottomCell = GetBottomCell(cellIndex, currentLiveCells, currentDeadCells);

                //--skip bottom left as it's already been set or out of grid
            }
        }

        private bool GetOutOfGridCells(int cellIndex, SurroundingCells surroundingCells, ref bool checkLeft, ref bool checkRight,
            ref bool checkTop, ref bool checkBottom)
        {
            if (OnLeftColumn(cellIndex))
            {
                surroundingCells.TopLeftCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.TopLeft);
                surroundingCells.LeftCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.Left);
                surroundingCells.BottomLeftCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.BottomLeft);
                checkLeft = false;
            }
            else if (OnRightColumn(cellIndex))
            {
                surroundingCells.TopRightCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.TopRight);
                surroundingCells.RightCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.Right);
                surroundingCells.BottomRightCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.BottomRight);
                checkRight = false;
            }

            if (OnTopRow(cellIndex))
            {
                surroundingCells.TopLeftCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.TopLeft);
                surroundingCells.TopCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.Top);
                surroundingCells.TopRightCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.TopRight);
                checkTop = false;
            }
            else if (OnBottomRow(cellIndex))
            {
                surroundingCells.BottomLeftCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.BottomLeft);
                surroundingCells.BottomCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.Bottom);
                surroundingCells.BottomRightCell = GridCell.MakeOutOfGridCell(cellIndex, RelativePosition.BottomRight);
                checkBottom = false;
            }

            return checkLeft;
        }

        private GridCell GetBottomLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var bottomLeftCellIndex = cellIndex + _numberOfRowsAndColumns - 1;
            if (currentLiveCells.ContainsKey(bottomLeftCellIndex))
            {
                return currentLiveCells[bottomLeftCellIndex];
            }

            if (currentDeadCells.ContainsKey(bottomLeftCellIndex))
            {
                return currentDeadCells[bottomLeftCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomLeftCellIndex, RelativePosition.BottomLeft);
        }

        private GridCell GetLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var leftCellIndex = cellIndex - 1;
            if (currentLiveCells.ContainsKey(leftCellIndex))
            {
                return currentLiveCells[leftCellIndex];
            }

            if (currentDeadCells.ContainsKey(leftCellIndex))
            {
                return currentDeadCells[leftCellIndex];
            }

            return GridCell.MakeEmptyCell(leftCellIndex, RelativePosition.Left);
        }

        private GridCell GetTopLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var topLeftCellIndex = cellIndex - _numberOfRowsAndColumns - 1;
            if (currentLiveCells.ContainsKey(topLeftCellIndex))
            {
                return currentLiveCells[topLeftCellIndex];
            }

            if (currentDeadCells.ContainsKey(topLeftCellIndex))
            {
                return currentDeadCells[topLeftCellIndex];
            }

            return GridCell.MakeEmptyCell(topLeftCellIndex, RelativePosition.TopLeft);
        }

        private GridCell GetTopCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var topCellIndex = cellIndex - _numberOfRowsAndColumns;
            if (currentLiveCells.ContainsKey(topCellIndex))
            {
                return currentLiveCells[topCellIndex];
            }

            if (currentDeadCells.ContainsKey(topCellIndex))
            {
                return currentDeadCells[topCellIndex];
            }

            return GridCell.MakeEmptyCell(topCellIndex, RelativePosition.Top);
        }

        private GridCell GetTopRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var topRightCellIndex = cellIndex - _numberOfRowsAndColumns + 1;
            if (currentLiveCells.ContainsKey(topRightCellIndex))
            {
                return currentLiveCells[topRightCellIndex];
            }

            if (currentDeadCells.ContainsKey(topRightCellIndex))
            {
                return currentDeadCells[topRightCellIndex];
            }

            return GridCell.MakeEmptyCell(topRightCellIndex, RelativePosition.TopRight);
        }

        private GridCell GetRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var rightCellIndex = cellIndex + 1;
            if (currentLiveCells.ContainsKey(rightCellIndex))
            {
                return currentLiveCells[rightCellIndex];
            }

            if (currentDeadCells.ContainsKey(rightCellIndex))
            {
                return currentDeadCells[rightCellIndex];
            }

            return GridCell.MakeEmptyCell(rightCellIndex, RelativePosition.Right);
        }

        private GridCell GetBottomRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var bottomRightCellIndex = cellIndex + _numberOfRowsAndColumns + 1;
            if (currentLiveCells.ContainsKey(bottomRightCellIndex))
            {
                return currentLiveCells[bottomRightCellIndex];
            }

            if (currentDeadCells.ContainsKey(bottomRightCellIndex))
            {
                return currentDeadCells[bottomRightCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomRightCellIndex, RelativePosition.BottomRight);
        }

        private GridCell GetBottomCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells, Dictionary<int, BioCell> currentDeadCells)
        {
            var bottomCellIndex = cellIndex + _numberOfRowsAndColumns;
            if (currentLiveCells.ContainsKey(bottomCellIndex))
            {
                return currentLiveCells[bottomCellIndex];
            }

            if (currentDeadCells.ContainsKey(bottomCellIndex))
            {
                return currentDeadCells[bottomCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomCellIndex, RelativePosition.Bottom);
        }

        private bool OnTopRow(int cellIndex)
        {
            return cellIndex < _numberOfRowsAndColumns;
        }

        private bool OnBottomRow(int cellIndex)
        {
            return cellIndex >= (_numberOfRowsAndColumns * _numberOfRowsAndColumns) - _numberOfRowsAndColumns;
        }

        private bool OnRightColumn(int cellIndex)
        {
            return cellIndex % _numberOfRowsAndColumns == (_numberOfRowsAndColumns - 1);
        }


        private bool OnLeftColumn(int cellIndex)
        {
            return cellIndex % _numberOfRowsAndColumns == 0;
        }
    }
}