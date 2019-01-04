using System.Collections.Generic;

namespace Logic
{
    public interface ICellRegrowthCalculator
    {
        CellRegrowthResult CalculateCellRegrowth(Dictionary<int, BioCell> currentDeadCells,
            Dictionary<int, BioCell> currentLiveCells, List<IPlayer> players);
    }
}