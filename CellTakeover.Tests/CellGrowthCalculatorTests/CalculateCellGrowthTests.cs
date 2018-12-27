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
        private readonly CellGrowthCalculator _cellGrowthCalculator = new CellGrowthCalculator();
        private readonly ISurroundingCellCalculator _surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>().Object;

        [TestMethod]
        public void It_Gets_New_Live_Cells_Calculated_From_Empty_Ones_Using_The_Players_Growth_Scorecard()
        {
            //--arrange
            var growthScorecard = new GrowthScorecard();
            growthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft] = 100;
            growthScorecard.GrowthChanceDictionary[RelativePosition.Top] = 100;
            var player = new Player("name", new Color(), 1, "A", _cellGrowthCalculator, _surroundingCellCalculatorMock);
            player.GrowthScorecard = growthScorecard;
            var bioCell = new BioCell(player, 1, new Color(), _surroundingCellCalculatorMock);

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
            var actualResult = _cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualResult.NewLiveCells.Count.ShouldBe(2);
            actualResult.NewLiveCells.ShouldContain(x => x.CellIndex == emptyIndex1);
            actualResult.NewLiveCells.ShouldContain(x => x.CellIndex == emptyIndex2);
            actualResult.NewDeadCells.ShouldBeEmpty();
        }

        [TestMethod]
        public void The_Cell_May_Die_If_It_Is_Surrounded_And_The_Player_Has_The_Minimum_Number_Of_Live_Cells()
        {
            //--arrange
            var cellGrowthCalculator = new CellGrowthCalculator();
            var surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>().Object;

            var player = new Player("name", new Color(), 1, "A", cellGrowthCalculator, surroundingCellCalculatorMock);
            player.LiveCells = CellGrowthCalculator.MinimumLiveCellsForCellDeath;
            var growthScorecard = new GrowthScorecard {DeathChanceForStarvedCells = 100};
            player.GrowthScorecard = growthScorecard;
            var bioCell = new BioCell(player, 1, new Color(), surroundingCellCalculatorMock);

            var surroundingCells = CreateSurroundingCellsWithAllBioCells(player);

            //--act
            var actualResult = cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualResult.NewDeadCells.ShouldContain(bioCell);
            bioCell.Player.DeadCells.ShouldBe(1);
        }

        [TestMethod]
        public void The_Cell_May_Die_At_Random_If_The_Player_Has_The_Minimum_Number_Of_Live_Cells()
        {
            //--arrange
            var cellGrowthCalculator = new CellGrowthCalculator();
            var surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>().Object;

            var player = new Player("name", new Color(), 1, "A", cellGrowthCalculator, surroundingCellCalculatorMock);
            player.LiveCells = CellGrowthCalculator.MinimumLiveCellsForCellDeath;
            var growthScorecard = new GrowthScorecard { DeathChanceForStarvedCells = 0 };
            player.GrowthScorecard = growthScorecard;
            player.GrowthScorecard.HealthyCellDeathChancePercentage = 100;
            var bioCell = new BioCell(player, 1, new Color(), surroundingCellCalculatorMock);

            var surroundingCells = CreateSurroundingCellsWithAllBioCells(player);

            //--act
            var actualResult = cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualResult.NewDeadCells.ShouldContain(bioCell);
            bioCell.Player.DeadCells.ShouldBe(1);
        }

        [TestMethod]
        public void A_Cell_Cannot_Die_If_A_Player_Has_Less_Than_The_Minimum_Number_Of_Live_Cells()
        {
            //--arrange
            var cellGrowthCalculator = new CellGrowthCalculator();
            var surroundingCellCalculatorMock = new Mock<ISurroundingCellCalculator>().Object;

            var player = new Player("name", new Color(), 1, "A", cellGrowthCalculator, surroundingCellCalculatorMock);
            player.LiveCells = CellGrowthCalculator.MinimumLiveCellsForCellDeath - 1;
            var growthScorecard = new GrowthScorecard { DeathChanceForStarvedCells = 100 };
            player.GrowthScorecard = growthScorecard;
            player.GrowthScorecard.HealthyCellDeathChancePercentage = 100;
            var bioCell = new BioCell(player, 1, new Color(), surroundingCellCalculatorMock);

            var surroundingCells = CreateSurroundingCellsWithAllBioCells(player);

            //--act
            var actualResult = cellGrowthCalculator.CalculateCellGrowth(bioCell, player, surroundingCells);

            //--assert
            actualResult.NewDeadCells.Count.ShouldBe(0);
        }

        private SurroundingCells CreateSurroundingCellsWithAllBioCells(Player player)
        {
            var surroundingCells = new SurroundingCells
            {
                TopLeftCell = CreateBioCell(player, 2, _surroundingCellCalculatorMock),
                TopCell = CreateBioCell(player, 3, _surroundingCellCalculatorMock),
                TopRightCell = CreateBioCell(player, 4, _surroundingCellCalculatorMock),
                RightCell = CreateBioCell(player, 5, _surroundingCellCalculatorMock),
                BottomRightCell = CreateBioCell(player, 6, _surroundingCellCalculatorMock),
                BottomCell = CreateBioCell(player, 7, _surroundingCellCalculatorMock),
                BottomLeftCell = CreateBioCell(player, 8, _surroundingCellCalculatorMock),
                LeftCell = CreateBioCell(player, 9, _surroundingCellCalculatorMock)
            };
            return surroundingCells;
        }

        private static BioCell CreateBioCell(Player player, int cellIndex, ISurroundingCellCalculator surroundingCellCalculatorMock)
        {
            return new BioCell(player, cellIndex, player.Color, surroundingCellCalculatorMock);
        }
    }
}
