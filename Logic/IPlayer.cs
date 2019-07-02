using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace Logic
{
    public interface IPlayer : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsHuman { get; }
        bool IsLocalPlayer(List<string> userName);
        int AvailableMutationPoints { get; set; }
        int ActionPoints { get; set; }
        Color Color { get; set; }
        string PlayerId { get; set; }
        int LiveCells { get; set; }
        int DeadCells { get; set; }
        int RegeneratedCells { get; set; }
        int LostDeadCells { get; set; }
        int StolenDeadCells { get; set; }
        int TotalRegrownCells { get; }
        int GrownCells { get; set; }
        int PerishedCells { get; set; }
        int FungicidalKills { get; set; }
        int SpentMutationPoints { get; set; }
        GrowthScorecard GrowthScorecard { get; set; }
        string TopLeftGrowthChance { get; }
        string TopGrowthChance { get; }
        string TopRightGrowthChance { get; }
        string RightGrowthChance { get; }
        string BottomRightGrowthChance { get; }
        string BottomGrowthChance { get; }
        string BottomLeftGrowthChance { get;  }
        string LeftGrowthChance { get; }
        //TODO Will eventually show the skill level next to each skill. These don't come back from the API yet.
        float HyperMutationSkillLevel { get; set; }
        float AntiApoptosisSkillLevel { get; set; }
        float RegenerationSkillLevel { get; set; }
        float BuddingSkillLevel { get; set; }
        float MycotoxinsSkillLevel { get; set; }
        float HydrophiliaSkillLevel { get; set; }
        float SporesSkillLevel { get; set; }
        bool HasPointsToSpend { get; }
        void IncreaseHypermutation();
        void DecreaseApoptosisChance();
        void IncreaseBudding();
        void IncreaseRegeneration();
        void IncreaseMycotoxicity();
        void IncreaseHydrophilia();
        void IncreaseSpores();
        void UseEyeDropper();
        void UseDeadCell();
        void UseIncreaseLighting();
        void UseDecreaseLighting();
    }
}