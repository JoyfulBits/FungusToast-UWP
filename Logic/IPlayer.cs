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
        float TopLeftGrowthChance { get; }
        float TopGrowthChance { get; }
        float TopRightGrowthChance { get; }
        float RightGrowthChance { get; }
        float BottomRightGrowthChance { get; }
        float BottomGrowthChance { get; }
        float BottomLeftGrowthChance { get;  }
        float LeftGrowthChance { get; }
        float HyperMutationSkillLevel { get; set; }
        float AntiApoptosisSkillLevel { get; set; }
        float RegenerationSkillLevel { get; set; }
        float BuddingSkillLevel { get; set; }
        float MycotoxinsSkillLevel { get; set; }
        void IncreaseHypermutation();
        void DecreaseApoptosisChance();
        void IncreaseBudding();
        void IncreaseRegeneration();
        void IncreaseMycotoxicity();
        bool IsCurrentPlayer(string userName);
    }
}