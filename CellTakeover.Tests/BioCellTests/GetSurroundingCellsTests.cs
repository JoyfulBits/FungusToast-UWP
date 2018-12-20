using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace CellTakeover.Tests.BioCellTests
{
    [TestClass]
    public class GetSurroundingCellsTests
    {
        [TestMethod]
        public void The_Surrounding_Cell_Is_An_EdgeCell_If_At_The_Edge_Of_The_Grid()
        {
            //--arrange
            var bioCell = new BioCell(0, 0, Colors.Brown);
            var liveCells = new Dictionary<int, BioCell>();

            //--act
            var actualSurroundingCells = bioCell.GetSurroundingCells(liveCells);

            //--assert
            actualSurroundingCells.LeftCell.OutOfGridCell.ShouldBe(true);
        }
    }
}
