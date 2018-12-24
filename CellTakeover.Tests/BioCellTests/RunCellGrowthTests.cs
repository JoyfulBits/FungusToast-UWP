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

            var expectedSurroundingCells = new SurroundingCells();
            BioCell actualBioCellInSurroundingCellCalculation = null;
            Dictionary<int, BioCell> actualLiveCellsInSurroundingCellCalculation = null;
            surroundingCellCalculatorMock
                .Setup(x => x.GetSurroundingCells(It.IsAny<BioCell>(), It.IsAny<Dictionary<int, BioCell>>()))
                .Returns(expectedSurroundingCells)
                .Callback<BioCell, Dictionary<int, BioCell>>((i, o) =>
                {
                    actualBioCellInSurroundingCellCalculation = i;
                    actualLiveCellsInSurroundingCellCalculation = o;
                });

            var expectedNewCells = new List<BioCell>();
            BioCell capturedBioCell = null;
            SurroundingCells capturedSurroundingCells = null;
            mockPlayer.Setup(mock => mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<SurroundingCells>()))
                .Returns(expectedNewCells)
                .Callback<BioCell, SurroundingCells>((i, o) =>
                {
                    capturedBioCell = i;
                    capturedSurroundingCells = o;
                });

            //--act
            var actualNewBioCells = bioCell.RunCellGrowth(liveCells);

            //--assert
            actualBioCellInSurroundingCellCalculation.ShouldBeSameAs(bioCell);
            actualLiveCellsInSurroundingCellCalculation.ShouldBeSameAs(liveCells);
            capturedBioCell.ShouldBeSameAs(bioCell);
            capturedSurroundingCells.ShouldBeSameAs(expectedSurroundingCells);
            actualNewBioCells.ShouldBeSameAs(expectedNewCells);
        }
    }
}
