using System.Collections.Generic;

namespace Logic
{
    public interface ICellGrowthCalculator
    {
        CellGrowthResult CalculateCellGrowth(BioCell cell, IPlayer player, SurroundingCells surroundingCells);
    }
}