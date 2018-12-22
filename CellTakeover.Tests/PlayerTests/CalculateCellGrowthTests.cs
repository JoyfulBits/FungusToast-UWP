using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.PlayerTests
{
    [TestClass]
    public class CalculateCellGrowthTests
    {
        private Player _player;
        private Mock<ICellGrowthCalculator> _cellGrowthCalculatorMock;

        [TestMethod]
        public void It_Checks_For_Cell_Growth_Into_Empty_Cells()
        {
            //--arrange
            _cellGrowthCalculatorMock = new Mock<ICellGrowthCalculator>();
            var expectedCells = new List<BioCell>();
            BioCell capturedBioCell = null;
            Player capturedPlayer = null;
            SurroundingCells capturedSurroundingCells = null;
            _cellGrowthCalculatorMock.Setup(mock =>
                    mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<Player>(), It.IsAny<SurroundingCells>()))
                .Returns(expectedCells)
                .Callback<BioCell, Player, SurroundingCells>((w, x, y) =>
                {
                    capturedBioCell = w;
                    capturedPlayer = x;
                    capturedSurroundingCells = y;
                });
            _player = new Player("player 1", new Color(), 1, _cellGrowthCalculatorMock.Object);

            var cell = new BioCell(_player, 0, _player.Color);
            var surroundingCells = new SurroundingCells
            {
                TopLeftCell = GridCell.MakeEmptyCell(0, RelativePosition.TopLeft),
                TopCell = GridCell.MakeEmptyCell(0, RelativePosition.Top),
                TopRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.TopRight),
                RightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Right),
                BottomRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomRight),
                BottomCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Bottom),
                BottomLeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomLeft),
                LeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Left)
            };

            //--act
            var newCells =_player.CalculateCellGrowth(cell, surroundingCells);

            //--assert
            capturedBioCell.ShouldBeSameAs(cell);
            capturedPlayer.ShouldBeSameAs(_player);
            capturedSurroundingCells.ShouldBeSameAs(surroundingCells);
            newCells.ShouldBeSameAs(expectedCells);
        }
    }
}
