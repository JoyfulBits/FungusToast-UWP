namespace Logic
{
    public class MutationOptionGenerator
    {
        public MutationOptionGenerator(float mutationPercentageChancePerAttributePoint,
            float cornerGrowthChancePerAttributePoint,
            float reducedApoptosisPercentagePerAttributePoint,
            float regenerationChancePerAttributePoint,
            float mycotoxinFungicideChancePerAttributePoint)
        {
            MutationPercentageChancePerAttributePoint = mutationPercentageChancePerAttributePoint;
            CornerGrowthChancePerAttributePoint = cornerGrowthChancePerAttributePoint;
            ReducedApoptosisPercentagePerAttributePoint = reducedApoptosisPercentagePerAttributePoint;
            RegenerationChancePerAttributePoint = regenerationChancePerAttributePoint;
            MycotoxinFungicideChancePerAttributePoint = mycotoxinFungicideChancePerAttributePoint;
        }
       
        public float MutationPercentageChancePerAttributePoint { get; }
        public float CornerGrowthChancePerAttributePoint { get; }
        public float ReducedApoptosisPercentagePerAttributePoint { get; }
        public float RegenerationChancePerAttributePoint { get; }
        public float MycotoxinFungicideChancePerAttributePoint { get; }

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

    }
}