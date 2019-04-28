using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace Logic
{
    public interface IPlayer : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsHuman { get; }
        int AvailableMutationPoints { get; set; }
        Color Color { get; set; }
        string PlayerId { get; set; }
        int LiveCells { get; set; }
        int DeadCells { get; set; }
        int RegrownCells { get; set; }
        GrowthScorecard GrowthScorecard { get; set; }
        double TopLeftGrowthChance { get; }
        double TopGrowthChance { get; }
        double TopRightGrowthChance { get; }
        double RightGrowthChance { get; }
        double BottomRightGrowthChance { get; }
        double BottomGrowthChance { get; }
        double BottomLeftGrowthChance { get;  }
        double LeftGrowthChance { get; }
        double HyperMutationSkillLevel { get; set; }
        double AntiApoptosisSkillLevel { get; set; }
        double RegenerationSkillLevel { get; set; }
        double BuddingSkillLevel { get; set; }
        double MycotoxinsSkillLevel { get; set; }
        void IncreaseHypermutation();
        void DecreaseApoptosisChance();
        void IncreaseBudding();
        void IncreaseRegeneration();
        bool IsCurrentPlayer(string userName);
    }
}