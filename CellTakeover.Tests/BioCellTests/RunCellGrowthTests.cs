using System;
using System.Collections.Generic;
using Windows.Services.Maps;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.BioCellTests
{
    [TestClass]
    public class RunCellGrowthTests
    {
        private Mock<ISurroundingCellCalculator> _surroundingCellCalculatorMock;
        private Mock<IPlayer> _playerMock;

        [TestInitialize]
        public void SetUp()
        {
            _surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>();
            _playerMock = new Mock<IPlayer>();
        }

        [TestMethod]
        public void It_Calculates_The_New_Cells_From_The_Surrounding_Cells_And_Player()
        {
            //--arrange
            var bioCell = new BioCell(_playerMock.Object, 1, Colors.Brown, _surroundingCellCalculatorMock.Object);
            var liveCells = new Dictionary<int, BioCell>();
            var deadCells = new Dictionary<int, BioCell>();

            var expectedSurroundingCells = new SurroundingCells();
            BioCell actualBioCellInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualLiveCellsInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualDeadCellsInSurroundingCellCalculation = null;
            _surroundingCellCalculatorMock
                .Setup(x => x.GetSurroundingCells(It.IsAny<BioCell>(), It.IsAny<Dictionary<int, BioCell>>(), It.IsAny<Dictionary<int, BioCell>>()))
                .Returns(expectedSurroundingCells)
                .Callback<BioCell, Dictionary<int, BioCell>, Dictionary<int, BioCell>>((i, o, x) =>
                {
                    actualBioCellInSurroundingCellCalculation = i;
                    actualLiveCellsInSurroundingCellCalculation = o;
                    actualDeadCellsInSurroundingCellCalculation = x;
                });

            var expectedCellGrowthResult = new CellGrowthResult(new List<BioCell>(), new List<BioCell>());
            BioCell capturedBioCell = null;
            SurroundingCells capturedSurroundingCells = null;
            _playerMock.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
                .Returns(expectedCellGrowthResult)
                .Callback<BioCell, SurroundingCells>((i, o) =>
                {
                    capturedBioCell = i;
                    capturedSurroundingCells = o;
                });

            //--act
            var actualCellGrowthResult = bioCell.RunCellGrowth(liveCells, deadCells);

            //--assert
            actualBioCellInSurroundingCellCalculation.ShouldBeSameAs(bioCell);
            actualLiveCellsInSurroundingCellCalculation.ShouldBeSameAs(liveCells);
            actualDeadCellsInSurroundingCellCalculation.ShouldBeSameAs(deadCells);

            capturedBioCell.ShouldBeSameAs(bioCell);
            capturedSurroundingCells.ShouldBeSameAs(expectedSurroundingCells);
            actualCellGrowthResult.ShouldBeSameAs(expectedCellGrowthResult);
        }

        [TestMethod]
        public void It_Checks_For_Dead_Cell_Regeneration()
        {
            //--arrange
            var bioCell = new BioCell(_playerMock.Object, 1, Colors.Brown, _surroundingCellCalculatorMock.Object);
            var liveCells = new Dictionary<int, BioCell>();
            var deadCells = new Dictionary<int, BioCell>();

            var expectedSurroundingCells = new SurroundingCells();
            BioCell actualBioCellInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualLiveCellsInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualDeadCellsInSurroundingCellCalculation = null;
            _surroundingCellCalculatorMock
                .Setup(x => x.GetSurroundingCells(It.IsAny<BioCell>(), It.IsAny<Dictionary<int, BioCell>>(), It.IsAny<Dictionary<int, BioCell>>()))
                .Returns(expectedSurroundingCells)
                .Callback<BioCell, Dictionary<int, BioCell>, Dictionary<int, BioCell>>((i, o, x) =>
                {
                    actualBioCellInSurroundingCellCalculation = i;
                    actualLiveCellsInSurroundingCellCalculation = o;
                    actualDeadCellsInSurroundingCellCalculation = x;
                });

            var expectedCellGrowthResult = new CellGrowthResult(new List<BioCell>(), new List<BioCell>());
            BioCell capturedBioCell = null;
            SurroundingCells capturedSurroundingCells = null;
            _playerMock.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
                .Returns(expectedCellGrowthResult)
                .Callback<BioCell, SurroundingCells>((i, o) =>
                {
                    capturedBioCell = i;
                    capturedSurroundingCells = o;
                });

            //--act
            var actualCellGrowthResult = bioCell.RunCellGrowth(liveCells, deadCells);

            //--assert
            actualBioCellInSurroundingCellCalculation.ShouldBeSameAs(bioCell);
            actualLiveCellsInSurroundingCellCalculation.ShouldBeSameAs(liveCells);
            actualDeadCellsInSurroundingCellCalculation.ShouldBeSameAs(deadCells);

            capturedBioCell.ShouldBeSameAs(bioCell);
            capturedSurroundingCells.ShouldBeSameAs(expectedSurroundingCells);
            actualCellGrowthResult.ShouldBeSameAs(expectedCellGrowthResult);
        }
    }
}
