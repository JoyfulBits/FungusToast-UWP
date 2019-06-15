namespace Logic
{
    public class PassiveSkillsData
    {
        public PassiveSkillsData(float mutationPercentageChancePerAttributePoint,
            float cornerGrowthChancePerAttributePoint,
            float reducedApoptosisPercentagePerAttributePoint,
            float regenerationChancePerAttributePoint,
            float mycotoxinFungicideChancePerAttributePoint,
            float moistureGrowthBoostPerAttributePoint, 
            float sporesChancePerAttributePoint)
        {
            MutationPercentageChancePerAttributePoint = mutationPercentageChancePerAttributePoint;
            CornerGrowthChancePerAttributePoint = cornerGrowthChancePerAttributePoint;
            ReducedApoptosisPercentagePerAttributePoint = reducedApoptosisPercentagePerAttributePoint;
            RegenerationChancePerAttributePoint = regenerationChancePerAttributePoint;
            MycotoxinFungicideChancePerAttributePoint = mycotoxinFungicideChancePerAttributePoint;
            MoistureGrowthBoostPerAttributePoint = moistureGrowthBoostPerAttributePoint;
            SporesChancePerAttributePoint = sporesChancePerAttributePoint;
        }
       
        public float MutationPercentageChancePerAttributePoint { get; }
        public float CornerGrowthChancePerAttributePoint { get; }
        public float ReducedApoptosisPercentagePerAttributePoint { get; }
        public float RegenerationChancePerAttributePoint { get; }
        public float MycotoxinFungicideChancePerAttributePoint { get; }
        public float MoistureGrowthBoostPerAttributePoint { get; }
        public float SporesChancePerAttributePoint { get; }


        public string IncreaseMutationChanceMessage =>
            $"Increase chance of earning bonus mutation points by {MutationPercentageChancePerAttributePoint}%.";
        public string IncreaseCornerGrowthChanceMessage =>
            $"Increase of corner growth by {CornerGrowthChancePerAttributePoint}%.";

        public string DecreaseApoptosisChanceMessage =>
            $"Decrease chance of random cell death (apoptosis) by {ReducedApoptosisPercentagePerAttributePoint}%.";

        public string IncreaseRegrowthChanceMessage => 
            $"Increase chance of reviving adjacent dead cell by {RegenerationChancePerAttributePoint}%.";

        public string IncreaseMycotoxinFungicideChanceMessage =>
            $"Increase chance of killing an adjacent enemy cell by {MycotoxinFungicideChancePerAttributePoint}%.";

        public string IncreaseMoistureGrowthBoostMessage =>
            $"Increase chance of growing into adjacent moist cells by {MoistureGrowthBoostPerAttributePoint}%.";

        public string IncreaseSporesChanceMessage =>
            $"Increase chance of growth into random empty space by {SporesChancePerAttributePoint}%.";
    }
}