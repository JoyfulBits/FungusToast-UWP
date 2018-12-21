using System.Collections.Generic;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.BioCellTests
{
    [TestClass]
    public class GetSurroundingCellsTests
    {
        private Mock<IPlayer> _mockPlayer;

        [TestInitialize]
        public void SetUp()
        {
            _mockPlayer = new Mock<IPlayer>();
        }

        [TestMethod]
        public void The_Surrounding_Left_Cells_Are_An_EdgeCell_If_At_The_Left_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
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
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
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
            var bioCell = new BioCell(_mockPlayer.Object, GameSettings.NumberOfColumnsAndRows - 1, Colors.Brown);
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
            var bioCell = new BioCell(_mockPlayer.Object, GameSettings.NumberOfCells - 1, Colors.Brown);
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
            var currentBioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 0, new Color());
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

            var bioCell = new BioCell(_mockPlayer.Object, secondRowSecondColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 0, new Color());
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

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top()
        {
            //--arrange
            var secondRowFirstColumnIndex = GameSettings.NumberOfColumnsAndRows;

            var bioCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 0, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.TopCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, topCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top_Right()
        {
            //--arrange
            var secondRowFirstColumnIndex = GameSettings.NumberOfColumnsAndRows;

            var bioCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 1, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.TopRightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, topRightCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Right()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 1, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.RightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, rightCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom_Right()
        {
            //--arrange
            var secondRowSecondColumnIndex = GameSettings.NumberOfColumnsAndRows + 1;

            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowSecondColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.BottomRightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomRightCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom()
        {
            //--arrange
            var secondRowFirstColumnIndex = GameSettings.NumberOfColumnsAndRows;

            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.BottomCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom_Left()
        {
            //--arrange
            var secondRowFirstColumnIndex = GameSettings.NumberOfColumnsAndRows;

            var bioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.BottomLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomLeftCellShouldBeEmpty: false);
        }

        //[TestMethod]
        //public void It_Calculates_The_New_Cells()
        //{
        //    //--arrange
        //    var bioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
        //    var growthCalculatorMock = new Mock<ICellGrowthCalculator>();
        //    bioCell.CellGrowthCalculator = growthCalculatorMock.Object;
        //    growthCalculatorMock.Setup(x => x.)

        //    //--act
        //    var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

        //    //--assert
        //    actualSurroundingCells.GrowthPattern.ShouldBeEqual();
        //    AssertAllEmpty(actualSurroundingCells, bottomLeftCellShouldBeEmpty: false);
        //}

        private void AssertAllEmpty(SurroundingCells actualSurroundingCells, 
            bool topLeftCellShouldBeEmpty = true,
            bool leftCellShouldBeEmpty = true,
            bool topCellShouldBeEmpty = true,
            bool topRightCellShouldBeEmpty = true,
            bool rightCellShouldBeEmpty = true,
            bool bottomRightCellShouldBeEmpty = true,
            bool bottomCellShouldBeEmpty = true,
            bool bottomLeftCellShouldBeEmpty = true)
        {
            if (leftCellShouldBeEmpty)
            {
                actualSurroundingCells.LeftCell.LiveCell.ShouldBeFalse();
            }

            if (topLeftCellShouldBeEmpty)
            {
                actualSurroundingCells.TopLeftCell.LiveCell.ShouldBeFalse();
            }

            if (topCellShouldBeEmpty)
            {
                actualSurroundingCells.TopCell.LiveCell.ShouldBeFalse();
            }

            if (topRightCellShouldBeEmpty)
            {
                actualSurroundingCells.TopRightCell.LiveCell.ShouldBeFalse();
            }

            if (rightCellShouldBeEmpty)
            {
                actualSurroundingCells.RightCell.LiveCell.ShouldBeFalse();
            }

            if (bottomRightCellShouldBeEmpty)
            {
                actualSurroundingCells.BottomRightCell.LiveCell.ShouldBeFalse();
            }

            if (bottomCellShouldBeEmpty)
            {
                actualSurroundingCells.BottomCell.LiveCell.ShouldBeFalse();
            }

            if (bottomLeftCellShouldBeEmpty)
            {
                actualSurroundingCells.BottomLeftCell.LiveCell.ShouldBeFalse();
            }
        }
    }
}
