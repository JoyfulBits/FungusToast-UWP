namespace ApiClient.Models
{
    public class PlayerState
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int MutationPoints { get; set; }
        public bool Human { get; set; }
        public int TopLeftGrowthChance { get; set; }
        public int TopGrowthChance { get; set; }
        public int TopRightGrowthChance { get; set; }
        public int RightGrowthChance { get; set; }
        public int BottomRightGrowthChance { get; set; }
        public int BottomGrowthChance { get; set; }
        public int BottomLeftGrowthChance { get; set; }
        public int LeftGrowthChance { get; set; }
        public int DeadCells { get; set; }
        public int LiveCells { get; set; }
        public int RegeneratedCells { get; set; }
        public int HyperMutationSkillLevel { get; set; }
        public int AntiApoptosisSkillLevel { get; set; }
        public int RegenerationSkillLevel { get; set; }
        public int BuddingSkillLevel { get; set; }
        public int MycotoxinsSkillLevel { get; set; }
        public double ApoptosisChancePercentage { get; set; }
        public double StarvedCellDeathChancePercentage { get; set; }
        public double MutationChancePercentage { get; set; }
        public double RegenerationChancePercentage { get; set; }
        public double MycotoxinFungicideChancePercentage { get; set; }
        public string Status { get; set; }
    }
}