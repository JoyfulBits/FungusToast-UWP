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
        private SurroundingCells _capturedSurroundingCells = null;

        [TestInitialize]
        public void SetUp()
        {
            _mockPlayer = new Mock<IPlayer>();

            _mockPlayer.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
                .Returns(new List<BioCell>())
                .Callback<BioCell, SurroundingCells>((i, o) => {
                    _capturedSurroundingCells = o;
                });
        }

        [TestMethod]
        public void The_Surrounding_Left_Cells_Are_An_EdgeCell_If_At_The_Left_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.BottomLeftCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.LeftCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.TopLeftCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Top_Cells_Are_An_EdgeCell_If_At_The_Top_Row_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.TopLeftCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.TopCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Right_Cells_Are_An_EdgeCell_If_At_The_Right_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, GameSettings.NumberOfColumnsAndRows - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.RightCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.BottomRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Bottom_Cells_Are_An_EdgeCell_If_At_The_Bottom_Row_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, GameSettings.NumberOfCells - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.BottomRightCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.BottomCell.OutOfGrid.ShouldBe(true);
            _capturedSurroundingCells.BottomLeftCell.OutOfGrid.ShouldBe(true);
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
            currentBioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.LeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, leftCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.TopLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, topLeftCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.TopCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, topCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.TopRightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, topRightCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.RightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, rightCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.BottomRightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, bottomRightCellShouldBeEmpty: false);
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
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.BottomCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, bottomCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom_Left()
        {
            //--arrange
            var secondRowFirstColumnIndex = GameSettings.NumberOfColumnsAndRows;
            SurroundingCells _capturedSurroundingCells = null;
            _mockPlayer.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
                .Returns(new List<BioCell>())
                .Callback<BioCell, SurroundingCells>((i, o) => {
                    _capturedSurroundingCells = o;
                });

            var bioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            bioCell.RunCellGrowth(liveCells);

            //--assert
            _capturedSurroundingCells.BottomLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(_capturedSurroundingCells, bottomLeftCellShouldBeEmpty: false);
        }

        //[TestMethod]
        //public void It_Calculates_The_New_Cells()
        //{
        //    //--arrange
        //    var bioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
        //    BioCell capturedBioCell;
        //    SurroundingCells _capturedSurroundingCells;
        //    _mockPlayer.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
        //        .Returns(new List<BioCell>())
        //        .Callback<BioCell, SurroundingCells>((i, o) => {
        //            capturedBioCell = i;
        //            _capturedSurroundingCells = o;
        //        });

        //    //--act
        //    var actualNewBioCells = bioCell.RunCellGrowth(new Dictionary<int, BioCell>());

        //    //--assert
            
        //}

        private void AssertAllEmpty(SurroundingCells surroundingCells, 
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
                surroundingCells.LeftCell.LiveCell.ShouldBeFalse();
            }

            if (topLeftCellShouldBeEmpty)
            {
                surroundingCells.TopLeftCell.LiveCell.ShouldBeFalse();
            }

            if (topCellShouldBeEmpty)
            {
                surroundingCells.TopCell.LiveCell.ShouldBeFalse();
            }

            if (topRightCellShouldBeEmpty)
            {
                surroundingCells.TopRightCell.LiveCell.ShouldBeFalse();
            }

            if (rightCellShouldBeEmpty)
            {
                surroundingCells.RightCell.LiveCell.ShouldBeFalse();
            }

            if (bottomRightCellShouldBeEmpty)
            {
                surroundingCells.BottomRightCell.LiveCell.ShouldBeFalse();
            }

            if (bottomCellShouldBeEmpty)
            {
                surroundingCells.BottomCell.LiveCell.ShouldBeFalse();
            }

            if (bottomLeftCellShouldBeEmpty)
            {
                surroundingCells.BottomLeftCell.LiveCell.ShouldBeFalse();
            }
        }
    }
}
