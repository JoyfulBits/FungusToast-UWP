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
        private Mock<ISurroundingCellCalculator> _surroundingCellCalculatorMock;

        [TestMethod]
        public void It_Checks_For_Cell_Growth_Into_Empty_Cells()
        {
            //--arrange
            _cellGrowthCalculatorMock = new Mock<ICellGrowthCalculator>();
            _surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>();

            var expectedCellGrowthResult = new CellGrowthResult(new List<BioCell>(), new List<BioCell>());
            BioCell capturedBioCell = null;
            Player capturedPlayer = null;
            SurroundingCells capturedSurroundingCells = null;
            _cellGrowthCalculatorMock.Setup(mock =>
                    mock.CalculateCellGrowth(It.IsAny<BioCell>(), It.IsAny<IPlayer>(), It.IsAny<SurroundingCells>()))
                .Returns(expectedCellGrowthResult)
                .Callback<BioCell, Player, SurroundingCells>((w, x, y) =>
                {
                    capturedBioCell = w;
                    capturedPlayer = x;
                    capturedSurroundingCells = y;
                });
            _player = new Player("player 1", new Color(), 1, "A", 
                _cellGrowthCalculatorMock.Object, 
                _surroundingCellCalculatorMock.Object);

            var cell = new BioCell(_player, 0, _player.Color, _surroundingCellCalculatorMock.Object);
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
            var actualCellGrowthResult =_player.CalculateCellGrowth(cell, surroundingCells);

            //--assert
            capturedBioCell.ShouldBeSameAs(cell);
            capturedPlayer.ShouldBeSameAs(_player);
            capturedSurroundingCells.ShouldBeSameAs(surroundingCells);
            actualCellGrowthResult.ShouldBeSameAs(expectedCellGrowthResult);
        }
    }
}
