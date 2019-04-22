namespace ApiClient.Models
{
    public class PlayerState
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int MutationPoints { get; set; }
        public bool Human { get; set; }
        public double TopLeftGrowthChance { get; set; }
        public double TopGrowthChance { get; set; }
        public double TopRightGrowthChance { get; set; }
        public double RightGrowthChance { get; set; }
        public double BottomRightGrowthChance { get; set; }
        public double BottomGrowthChance { get; set; }
        public double BottomLeftGrowthChance { get; set; }
        public double LeftGrowthChance { get; set; }
        public int DeadCells { get; set; }
        public int LiveCells { get; set; }
        public int RegeneratedCells { get; set; }
        public int HyperMutationSkillLevel { get; set; }
        public int AntiApoptosisSkillLevel { get; set; }
        public int RegenerationSkillLevel { get; set; }
        public int BuddingSkillLevel { get; set; }
        public int MycotoxinsSkillLevel { get; set; }
        public double ApoptosisChance { get; set; }
        public double StarvedCellDeathChance { get; set; }
        public double MutationChance { get; set; }
        public double RegenerationChance { get; set; }
        public double MycotoxinFungicideChance { get; set; }
        public string Status { get; set; }
    }
}