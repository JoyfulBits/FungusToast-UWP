using System.Collections.Generic;

namespace Logic
{
    public interface ICellGrowthCalculator
    {
        List<BioCell> CalculateCellGrowth(BioCell cell, Player player, SurroundingCells surroundingCells);
    }
}