using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Logic.Tests.CellGrowthCalculatorTests
{
    [TestClass]
    public class CalculateCellGrowthTests
    {
        [TestMethod]
        public void It_Gets_New_Cells_Calculated_From_Empty_Ones_Using_Base_Growth_Rate()
        {
            //--arrange
            var cellGrowthCalculator = new CellGrowthCalculator();
            var growthScorecard = new GrowthScorecard
            {
                //--set to 100% so we get live cells for all empty cells
                BaseGrowthRatePercentage = 100
            };
            var player = new Player("name", new Color(), 1, cellGrowthCalculator);
            player.GrowthScorecard = growthScorecard;
            var bioCell = new BioCell(player, 1, new Color());

            

            var emptyIndex1 = 1;
            var emptyIndex2 = 2;
            var surroundingCells = new SurroundingCells
            {
                TopLeftCell = GridCell.MakeEmptyCell(emptyIndex1, RelativePosition.TopLeft),
                TopCell = GridCell.MakeEmptyCell(emptyIndex2, RelativePosition.Top),
                TopRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.TopRight),
                RightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Right),
                BottomRightCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomRight),
                BottomCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Bottom),
                BottomLeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.BottomLeft),
                LeftCell = GridCell.MakeOutOfGridCell(0, RelativePosition.Left)
            };

            //--act
            var actualNewCells = cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualNewCells.Count.ShouldBe(2);
            actualNewCells.ShouldContain(x => x.CellIndex == emptyIndex1);
            actualNewCells.ShouldContain(x => x.CellIndex == emptyIndex2);
        }
    }
}
