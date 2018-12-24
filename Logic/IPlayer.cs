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
        string CharacterSymbol { get; set; }
        int TotalCells { get; set; }
        int DeadCells { get; set; }
        BioCell MakeCell(int cellIndex);
        CellGrowthResult CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells);
    }
}