using System;
using System.Collections.Generic;
using Windows.UI;

namespace Logic
{
    public interface IPlayer
    {
        string Name { get; set; }
        Color Color { get; set; }
        int PlayerNumber { get; set; }
        BioCell MakeCell(int cellIndex);
        List<BioCell> CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells);
    }
}