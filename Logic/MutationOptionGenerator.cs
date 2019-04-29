using System.Collections.Generic;

namespace Logic
{
    public class MutationOptionGenerator
    {
        public static float AdditionalMutationPercentageChancePerAttributePoint = 3.0F;
        public static float AdditionalCornerGrowthChancePerAttributePoint = .4F;
        public static float ReducedApoptosisPercentagePerAttributePoint = .25F;
        public static float AdditionalRegenerationChancePerAttributePoint = .25F;
        public static float AdditionalMycotoxinFungicideChancePerAttributePoint = .25F;

        public static string IncreaseMutationChanceMessage =>
            $"Increase mutation chance by {AdditionalMutationPercentageChancePerAttributePoint}%.";
        public static string IncreaseCornerGrowthChanceMessage =>
            $"Increase of corner growth by {AdditionalCornerGrowthChancePerAttributePoint}%.";

        public static string DecreaseApoptosisChanceMessage =>
            $"Decrease chance of random cell death (apoptosis) by {ReducedApoptosisPercentagePerAttributePoint}%.";

        public static string IncreaseRegrowthChanceMessage => 
            $"Increase chance of reviving adjacent dead cell by {AdditionalRegenerationChancePerAttributePoint}%.";

        public static string IncreaseMycotoxinFungicideChanceMessage =>
            $"Increase chance of killing an adjacent enemy cell by {AdditionalMycotoxinFungicideChancePerAttributePoint}%.";

    }
}