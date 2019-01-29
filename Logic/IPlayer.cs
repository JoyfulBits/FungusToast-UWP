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
        string PlayerId { get; set; }
        int LiveCells { get; set; }
        int DeadCells { get; set; }
        int RegrownCells { get; set; }
        GrowthScorecard GrowthScorecard { get; set; }
        int TopLeftGrowthChance { get; }
        int TopGrowthChance { get; }
        int TopRightGrowthChance { get; }
        int RightGrowthChance { get; }
        int BottomRightGrowthChance { get; }
        int BottomGrowthChance { get; }
        int BottomLeftGrowthChance { get; }
        int LeftGrowthChance { get; }
        bool IsHuman { get; }
        BioCell MakeCell(int cellIndex);
        CellGrowthResult CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells);
        bool GetsFreeMutation();
        void IncreaseMutationChance();
        void DecreaseApoptosisChance();
        void IncreaseCornerGrowth();
        void IncreaseRegrowthChance();
        BioCell RegrowCell(BioCell deadCell);
    }
}