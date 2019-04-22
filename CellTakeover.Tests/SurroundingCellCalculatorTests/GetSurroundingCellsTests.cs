using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.SurroundingCellCalculatorTests
{
    [TestClass]
    public class GetSurroundingCellsTests
    {
        private Mock<IPlayer> _mockPlayer;
        private SurroundingCellCalculator _surroundingCellCalculator;
        private int _numberOfRowsAndColumns = 50;
        private int _numberOfCells;

        private Dictionary<int, BioCell> _dummyDictionary = new Dictionary<int, BioCell>();

        [TestInitialize]
        public void SetUp()
        {
            _numberOfCells = _numberOfRowsAndColumns * _numberOfRowsAndColumns;
            _surroundingCellCalculator = new SurroundingCellCalculator(_numberOfRowsAndColumns);
            _mockPlayer = new Mock<IPlayer>();
        }

        [TestMethod]
        public void The_Surrounding_Left_Cells_Are_An_EdgeCell_If_At_The_Left_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, _dummyDictionary, _dummyDictionary);

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
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.TopLeftCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.TopCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Right_Cells_Are_An_EdgeCell_If_At_The_Right_Column_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, _numberOfRowsAndColumns - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.TopRightCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.RightCell.OutOfGrid.ShouldBe(true);
            actualSurroundingCells.BottomRightCell.OutOfGrid.ShouldBe(true);
        }

        [TestMethod]
        public void The_Surrounding_Bottom_Cells_Are_An_EdgeCell_If_At_The_Bottom_Row_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(_mockPlayer.Object, _numberOfCells - 1, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

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
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(currentBioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.LeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, leftCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top_Left()
        {
            //--arrange
            var secondRowSecondColumnIndex = _numberOfRowsAndColumns + 1;

            var bioCell = new BioCell(_mockPlayer.Object, secondRowSecondColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 0, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.TopLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, topLeftCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top()
        {
            //--arrange
            var secondRowFirstColumnIndex = _numberOfRowsAndColumns;

            var bioCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 0, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.TopCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, topCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Top_Right()
        {
            //--arrange
            var secondRowFirstColumnIndex = _numberOfRowsAndColumns;

            var bioCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, 1, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

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
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.RightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, rightCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom_Right()
        {
            //--arrange
            var secondRowSecondColumnIndex = _numberOfRowsAndColumns + 1;

            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowSecondColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.BottomRightCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomRightCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom()
        {
            //--arrange
            var secondRowFirstColumnIndex = _numberOfRowsAndColumns;

            var bioCell = new BioCell(_mockPlayer.Object, 0, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, _dummyDictionary);

            //--assert
            actualSurroundingCells.BottomCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomCellShouldBeEmpty: false);
        }

        [TestMethod]
        public void It_Finds_Live_Cells_To_The_Bottom_Left()
        {
            //--arrange
            var secondRowFirstColumnIndex = _numberOfRowsAndColumns;
  
            var bioCell = new BioCell(_mockPlayer.Object, 1, Colors.Brown);
            var expectedCell = new BioCell(_mockPlayer.Object, secondRowFirstColumnIndex, new Color());
            var liveCells = new Dictionary<int, BioCell>
            {
                {expectedCell.CellIndex, expectedCell }
            };

            //--act
            var actualSurroundingCells = _surroundingCellCalculator.GetSurroundingCells(bioCell, liveCells, new Dictionary<int, BioCell>());

            //--assert
            actualSurroundingCells.BottomLeftCell.ShouldBeSameAs(expectedCell);
            AssertAllEmpty(actualSurroundingCells, bottomLeftCellShouldBeEmpty: false);
        }

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
                surroundingCells.LeftCell.OrganicCell.ShouldBeFalse();
            }

            if (topLeftCellShouldBeEmpty)
            {
                surroundingCells.TopLeftCell.OrganicCell.ShouldBeFalse();
            }

            if (topCellShouldBeEmpty)
            {
                surroundingCells.TopCell.OrganicCell.ShouldBeFalse();
            }

            if (topRightCellShouldBeEmpty)
            {
                surroundingCells.TopRightCell.OrganicCell.ShouldBeFalse();
            }

            if (rightCellShouldBeEmpty)
            {
                surroundingCells.RightCell.OrganicCell.ShouldBeFalse();
            }

            if (bottomRightCellShouldBeEmpty)
            {
                surroundingCells.BottomRightCell.OrganicCell.ShouldBeFalse();
            }

            if (bottomCellShouldBeEmpty)
            {
                surroundingCells.BottomCell.OrganicCell.ShouldBeFalse();
            }

            if (bottomLeftCellShouldBeEmpty)
            {
                surroundingCells.BottomLeftCell.OrganicCell.ShouldBeFalse();
            }
        }
    }
}
