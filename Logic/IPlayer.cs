using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace Logic
{
    public interface IPlayer : INotifyPropertyChanged
    {
        string Name { get; set; }
        int AvailableMutationPoints { get; set; }
        Color Color { get; set; }
        int PlayerNumber { get; set; }
        string PlayerSymbol { get; set; }
        int LiveCells { get; set; }
        int DeadCells { get; set; }
        int RegrownCells { get; set; }
        GrowthScorecard GrowthScorecard { get; set; }
        BioCell MakeCell(int cellIndex);
        CellGrowthResult CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells);
        bool GetsFreeMutation();
        void IncreaseMutationChance();
        void DecreaseHealthyCellDeathChance();
        void IncreaseCornerGrowth();
        void IncreaseRegrowthChance();
        BioCell RegrowCell(BioCell deadCell);
        void KillCell();
    }
}