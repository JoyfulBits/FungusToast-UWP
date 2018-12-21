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

        public BioCell(IPlayer player, int cellIndex, Color cellColor)
        {
            Player = player;
            CellIndex = cellIndex;
            CellColor = cellColor;
            OutOfGrid = false;
            LiveCell = true; 
        }

        public List<BioCell> RunCellGrowth(Dictionary<int, BioCell> currentLiveCells)
        {
            var surroundingCells = new SurroundingCells();
            var checkLeft = true;
            var checkTop = true;
            var checkRight = true;
            var checkBottom = true;

            checkLeft = GetOutOfGridCells(surroundingCells, ref checkLeft, ref checkRight, ref checkTop, ref checkBottom);

            GetInGridCells( surroundingCells, currentLiveCells, checkLeft, checkBottom, checkTop, checkRight);

            return Player.CalculateCellGrowth(this, surroundingCells);
        }

        private void GetInGridCells(SurroundingCells surroundingCells, 
            Dictionary<int, BioCell> currentLiveCells, bool checkLeft, 
            bool checkBottom, bool checkTop, bool checkRight)
        {
            if (checkLeft)
            {
                if (checkBottom)
                {
                    surroundingCells.BottomLeftCell = GetBottomLeftCell(currentLiveCells);
                }

                surroundingCells.LeftCell = GetLeftCell(currentLiveCells);

                if (checkTop)
                {
                    surroundingCells.TopLeftCell = GetTopLeftCell(currentLiveCells);
                }
            }

            if (checkTop)
            {
                //--skip top left cell as it's already been set or out of grid

                surroundingCells.TopCell = GetTopCell(currentLiveCells);

                if (checkRight)
                {
                    surroundingCells.TopRightCell = GetTopRightCell(currentLiveCells);
                }
            }

            if (checkRight)
            {
                //--skip top right cell as it's already been set or out of grid

                surroundingCells.RightCell = GetRightCell(currentLiveCells);

                if (checkBottom)
                {
                    surroundingCells.BottomRightCell = GetBottomRightCell(currentLiveCells);
                }
            }

            if (checkBottom)
            {
                //--skip bottom right cell as it's already been set or out of grid

                surroundingCells.BottomCell = GetBottomCell(currentLiveCells);

                //--skip bottom left as it's already been set or out of grid
            }
        }

        private bool GetOutOfGridCells(SurroundingCells surroundingCells, ref bool checkLeft, ref bool checkRight,
            ref bool checkTop, ref bool checkBottom)
        {
            if (OnLeftColumn())
            {
                surroundingCells.TopLeftCell = OutOfGridCell;
                surroundingCells.LeftCell = OutOfGridCell;
                surroundingCells.BottomLeftCell = OutOfGridCell;
                checkLeft = false;
            }
            else if (OnRightColumn())
            {
                surroundingCells.TopRightCell = OutOfGridCell;
                surroundingCells.RightCell = OutOfGridCell;
                surroundingCells.BottomRightCell = OutOfGridCell;
                checkRight = false;
            }

            if (OnTopRow())
            {
                surroundingCells.TopLeftCell = OutOfGridCell;
                surroundingCells.TopCell = OutOfGridCell;
                surroundingCells.TopRightCell = OutOfGridCell;
                checkTop = false;
            }
            else if (OnBottomRow())
            {
                surroundingCells.BottomLeftCell = OutOfGridCell;
                surroundingCells.BottomCell = OutOfGridCell;
                surroundingCells.BottomRightCell = OutOfGridCell;
                checkBottom = false;
            }

            return checkLeft;
        }

        private GridCell GetBottomLeftCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomLeftCellIndex = CellIndex + GameSettings.NumberOfColumnsAndRows - 1;
            if (currentLiveCells.ContainsKey(bottomLeftCellIndex))
            {
                return currentLiveCells[bottomLeftCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetLeftCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var leftCellIndex = CellIndex - 1;
            if (currentLiveCells.ContainsKey(leftCellIndex))
            {
                return currentLiveCells[leftCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetTopLeftCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var topLeftCellIndex = CellIndex - GameSettings.NumberOfColumnsAndRows - 1;
            if (currentLiveCells.ContainsKey(topLeftCellIndex))
            {
                return currentLiveCells[topLeftCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetTopCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var topCellIndex = CellIndex - GameSettings.NumberOfColumnsAndRows;
            if (currentLiveCells.ContainsKey(topCellIndex))
            {
                return currentLiveCells[topCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetTopRightCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var topRightCellIndex = CellIndex - GameSettings.NumberOfColumnsAndRows + 1;
            if (currentLiveCells.ContainsKey(topRightCellIndex))
            {
                return currentLiveCells[topRightCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetRightCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var rightCellIndex = CellIndex + 1;
            if (currentLiveCells.ContainsKey(rightCellIndex))
            {
                return currentLiveCells[rightCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetBottomRightCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomRightCellIndex = CellIndex + GameSettings.NumberOfColumnsAndRows + 1;
            if (currentLiveCells.ContainsKey(bottomRightCellIndex))
            {
                return currentLiveCells[bottomRightCellIndex];
            }

            return GridCell.EmptyCell;
        }

        private GridCell GetBottomCell(Dictionary<int, BioCell> currentLiveCells)
        {
            var bottomCellIndex = CellIndex + GameSettings.NumberOfColumnsAndRows;
            if (currentLiveCells.ContainsKey(bottomCellIndex))
            {
                return currentLiveCells[bottomCellIndex];
            }

            return GridCell.EmptyCell;
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