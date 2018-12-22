using System.Collections.Generic;

namespace Logic
{
    public class SurroundingCellCalculator : ISurroundingCellCalculator
    {
        public SurroundingCells GetSurroundingCells(BioCell bioCell, Dictionary<int, BioCell> currentLiveCells)
        {
            var surroundingCells = new SurroundingCells();
            var checkLeft = true;
            var checkTop = true;
            var checkRight = true;
            var checkBottom = true;
            checkLeft = GetOutOfGridCells(bioCell.CellIndex, surroundingCells, 
                ref checkLeft, ref checkRight, ref checkTop, ref checkBottom);

            GetInGridCells(bioCell.CellIndex, surroundingCells, currentLiveCells, checkLeft, checkBottom, checkTop, checkRight);

            return surroundingCells;
        }

        private void GetInGridCells(int cellIndex, SurroundingCells surroundingCells,
            Dictionary<int, BioCell> currentLiveCells, bool checkLeft,
            bool checkBottom, bool checkTop, bool checkRight)
        {
            if (checkLeft)
            {
                if (checkBottom)
                {
                    surroundingCells.BottomLeftCell = GetBottomLeftCell(cellIndex, currentLiveCells);
                }

                surroundingCells.LeftCell = GetLeftCell(cellIndex, currentLiveCells);

                if (checkTop)
                {
                    surroundingCells.TopLeftCell = GetTopLeftCell(cellIndex, currentLiveCells);
                }
            }

            if (checkTop)
            {
                //--skip top left cell as it's already been set or out of grid

                surroundingCells.TopCell = GetTopCell(cellIndex, currentLiveCells);

                if (checkRight)
                {
                    surroundingCells.TopRightCell = GetTopRightCell(cellIndex, currentLiveCells);
                }
            }

            if (checkRight)
            {
                //--skip top right cell as it's already been set or out of grid

                surroundingCells.RightCell = GetRightCell(cellIndex, currentLiveCells);

                if (checkBottom)
                {
                    surroundingCells.BottomRightCell = GetBottomRightCell(cellIndex, currentLiveCells);
                }
            }

            if (checkBottom)
            {
                //--skip bottom right cell as it's already been set or out of grid

                surroundingCells.BottomCell = GetBottomCell(cellIndex, currentLiveCells);

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

        private GridCell GetBottomLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomLeftCellIndex = cellIndex + GameSettings.NumberOfColumnsAndRows - 1;
            if (currentLiveCells.ContainsKey(bottomLeftCellIndex))
            {
                return currentLiveCells[bottomLeftCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomLeftCellIndex, RelativePosition.BottomLeft);
        }

        private GridCell GetLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var leftCellIndex = cellIndex - 1;
            if (currentLiveCells.ContainsKey(leftCellIndex))
            {
                return currentLiveCells[leftCellIndex];
            }

            return GridCell.MakeEmptyCell(leftCellIndex, RelativePosition.Left);
        }

        private GridCell GetTopLeftCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var topLeftCellIndex = cellIndex - GameSettings.NumberOfColumnsAndRows - 1;
            if (currentLiveCells.ContainsKey(topLeftCellIndex))
            {
                return currentLiveCells[topLeftCellIndex];
            }

            return GridCell.MakeEmptyCell(topLeftCellIndex, RelativePosition.TopLeft);
        }

        private GridCell GetTopCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var topCellIndex = cellIndex - GameSettings.NumberOfColumnsAndRows;
            if (currentLiveCells.ContainsKey(topCellIndex))
            {
                return currentLiveCells[topCellIndex];
            }

            return GridCell.MakeEmptyCell(topCellIndex, RelativePosition.Top);
        }

        private GridCell GetTopRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var topRightCellIndex = cellIndex - GameSettings.NumberOfColumnsAndRows + 1;
            if (currentLiveCells.ContainsKey(topRightCellIndex))
            {
                return currentLiveCells[topRightCellIndex];
            }

            return GridCell.MakeEmptyCell(topRightCellIndex, RelativePosition.TopRight);
        }

        private GridCell GetRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var rightCellIndex = cellIndex + 1;
            if (currentLiveCells.ContainsKey(rightCellIndex))
            {
                return currentLiveCells[rightCellIndex];
            }

            return GridCell.MakeEmptyCell(rightCellIndex, RelativePosition.Right);
        }

        private GridCell GetBottomRightCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomRightCellIndex = cellIndex + GameSettings.NumberOfColumnsAndRows + 1;
            if (currentLiveCells.ContainsKey(bottomRightCellIndex))
            {
                return currentLiveCells[bottomRightCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomRightCellIndex, RelativePosition.BottomRight);
        }

        private GridCell GetBottomCell(int cellIndex, Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomCellIndex = cellIndex + GameSettings.NumberOfColumnsAndRows;
            if (currentLiveCells.ContainsKey(bottomCellIndex))
            {
                return currentLiveCells[bottomCellIndex];
            }

            return GridCell.MakeEmptyCell(bottomCellIndex, RelativePosition.Bottom);
        }

        private bool OnTopRow(int cellIndex)
        {
            return cellIndex < GameSettings.NumberOfColumnsAndRows;
        }

        private bool OnBottomRow(int cellIndex)
        {
            return cellIndex >= GameSettings.NumberOfCells - GameSettings.NumberOfColumnsAndRows;
        }

        private bool OnRightColumn(int cellIndex)
        {
            return cellIndex % GameSettings.NumberOfColumnsAndRows == (GameSettings.NumberOfColumnsAndRows - 1);
        }


        private bool OnLeftColumn(int cellIndex)
        {
            return cellIndex % GameSettings.NumberOfColumnsAndRows == 0;
        }
    }
}