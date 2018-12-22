using System.Collections.Generic;

namespace Logic
{
    public interface ISurroundingCellCalculator
    {
        SurroundingCells GetSurroundingCells(BioCell bioCell, Dictionary<int, BioCell> currentLiveCells);
    }
}