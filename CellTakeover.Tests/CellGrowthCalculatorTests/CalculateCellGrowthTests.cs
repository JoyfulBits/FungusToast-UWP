using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.CellGrowthCalculatorTests
{
    [TestClass]
    public class CalculateCellGrowthTests
    {
        [TestMethod]
        public void It_Gets_New_Live_Cells_Calculated_From_Empty_Ones_Using_The_Players_Growth_Scorecard()
        {
            //--arrange
            var cellGrowthCalculator = new CellGrowthCalculator();
            var surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>().Object;
            var growthScorecard = new GrowthScorecard();
            growthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft] = 100;
            growthScorecard.GrowthChanceDictionary[RelativePosition.Top] = 100;
            var player = new Player("name", new Color(), 1, "A", cellGrowthCalculator, surroundingCellCalculatorMock);
            player.GrowthScorecard = growthScorecard;
            var bioCell = new BioCell(player, 1, new Color(), surroundingCellCalculatorMock);

            

            var emptyIndex1 = 1;
            var emptyIndex2 = 2;
            var surroundingCells = new SurroundingCells
            {
                //--100% chance
                TopLeftCell = GridCell.MakeEmptyCell(emptyIndex1, RelativePosition.TopLeft),
                //--100% chance
                TopCell = GridCell.MakeEmptyCell(emptyIndex2, RelativePosition.Top),
                TopRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.TopRight),
                RightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Right),
                BottomRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomRight),
                BottomCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Bottom),
                BottomLeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomLeft),
                LeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Left)
            };

            //--act
            var actualResult = cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualResult.NewLiveCells.Count.ShouldBe(2);
            actualResult.NewLiveCells.ShouldContain(x => x.CellIndex == emptyIndex1);
            actualResult.NewLiveCells.ShouldContain(x => x.CellIndex == emptyIndex2);
        }
    }
}
