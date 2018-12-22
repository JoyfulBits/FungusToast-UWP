using System.Collections.Generic;

namespace Logic
{
    public interface ICellGrowthCalculator
    {
        List<BioCell> CalculateCellGrowth(BioCell cell, GrowthScorecard growthScorecard, SurroundingCells surroundingCells);
    }
}