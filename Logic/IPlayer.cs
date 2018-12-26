using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace Logic
{
    public interface IPlayer : INotifyPropertyChanged
    {
        string Name { get; set; }
        Color Color { get; set; }
        int PlayerNumber { get; set; }
        string CharacterSymbol { get; set; }
        int TotalCells { get; set; }
        int DeadCells { get; set; }
        GrowthScorecard GrowthScorecard { get; set; }
        int MutationChancePercentage { get; set; }
        BioCell MakeCell(int cellIndex);
        CellGrowthResult CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells);
    }
}