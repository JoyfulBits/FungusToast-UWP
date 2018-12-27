using System.Collections.Generic;

namespace Logic
{
    public interface ICellRegrowthCalculator
    {
        List<BioCell> CalculateCellRegrowth(Dictionary<int, BioCell> currentDeadCells, Dictionary<int, BioCell> currentLiveCells);
    }
}