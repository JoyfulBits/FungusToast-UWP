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
        [TestMethod]
        public void It_Calculates_The_New_Cells_From_The_Surrounding_Cells_And_Player()
        {
            //--arrange
            var surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>();

            var mockPlayer = new Mock<IPlayer>();
            var bioCell = new BioCell(mockPlayer.Object, 1, Colors.Brown, surroundingCellCalculatorMock.Object);
            var liveCells = new Dictionary<int, BioCell>();
            var deadCells = new Dictionary<int, BioCell>();

            var expectedSurroundingCells = new SurroundingCells();
            BioCell actualBioCellInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualLiveCellsInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualDeadCellsInSurroundingCellCalculation = null;
            surroundingCellCalculatorMock
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
            mockPlayer.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
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
