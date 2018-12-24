using System.Collections.Generic;

namespace Logic
{
    public interface ICellGrowthCalculator
    {
        CellGrowthResult CalculateCellGrowth(BioCell cell, Player player, SurroundingCells surroundingCells);
    }
}