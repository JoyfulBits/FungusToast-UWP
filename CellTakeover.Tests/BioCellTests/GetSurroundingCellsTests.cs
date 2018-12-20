using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace CellTakeover.Tests.BioCellTests
{
    [TestClass]
    public class GetSurroundingCellsTests
    {
        [TestMethod]
        public void The_Surrounding_Left_Cells_Are_An_EdgeCell_If_At_The_Left_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(0, 0, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.BottomLeftCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.LeftCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.TopLeftCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Top_Cells_Are_An_EdgeCell_If_At_The_Top_Row_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(0, 0, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.TopLeftCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.TopCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Right_Cells_Are_An_EdgeCell_If_At_The_Right_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(0, GameSettings.NumberOfColumnsAndRows - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.RightCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.BottomRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Bottom_Cells_Are_An_EdgeCell_If_At_The_Bottom_Row_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(0, GameSettings.NumberOfCells - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.BottomRightCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.BottomCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.BottomLeftCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Left()
        {
            //--arrange
            var currentBioCell = new BioCell(0, 1, Colors.Brown);
            var expectedCell = new BioCell(0, 0, new Color());
            var liveCells = new Dictionary<int, BioCell> 
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = currentBioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.LeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, leftCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top_Left()
        {
            //--arrange
            var secondRowSecondColumnIndex = GameSettings.NumberOfColumnsAndRows + 1;

            var bioCell = new BioCell(0, secondRowSecondColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(0, 0, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.TopLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, topLeftCellShouldBeEmpty: false);
        }

        private void AssertAllEmpty(SurroundingCells actualSurroundingCells, bool topLeftCellShouldBeEmpty = true,
            bool leftCellShouldBeEmpty = true)
        {
            if (leftCellShouldBeEmpty)
            {
                actualSurroundingCells.TopLeftCell.ShouldBeSameAs(GridCell.EmptyCell);
            }

            if (topLeftCellShouldBeEmpty)
            {
                actualSurroundingCells.TopLeftCell.ShouldBeSameAs(GridCell.EmptyCell);
            }
            actualSurroundingCells.TopCell.ShouldBeSameAs(GridCell.EmptyCell);
            actualSurroundingCells.TopRightCell.ShouldBeSameAs(GridCell.EmptyCell);
            actualSurroundingCells.RightCell.ShouldBeSameAs(GridCell.EmptyCell);
            actualSurroundingCells.BottomRightCell.ShouldBeSameAs(GridCell.EmptyCell);
            actualSurroundingCells.BottomCell.ShouldBeSameAs(GridCell.EmptyCell);
            actualSurroundingCells.BottomLeftCell.ShouldBeSameAs(GridCell.EmptyCell);

        }
    }
}
