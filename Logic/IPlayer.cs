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
        //TODO These skill level fields aren't actually used yet, and don't actually come back from the API. Probably delete?
        float HyperMutationSkillLevel { get; set; }
        float AntiApoptosisSkillLevel { get; set; }
        float RegenerationSkillLevel { get; set; }
        float BuddingSkillLevel { get; set; }
        float MycotoxinsSkillLevel { get; set; }
        float HydrophiliaSkillLevel { get; set; }
        void IncreaseHypermutation();
        void DecreaseApoptosisChance();
        void IncreaseBudding();
        void IncreaseRegeneration();
        void IncreaseMycotoxicity();
        void IncreaseHydrophilia();
        bool IsLocalPlayer(List<string> userName);
    }
}