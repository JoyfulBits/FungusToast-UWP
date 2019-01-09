using System.Collections.Generic;

namespace Logic
{
    public class MutationOptionGenerator
    {
        public static int AdditionalMutationPercentageChancePerAttributePoint = 3;
        public static int AdditionalCornerGrowthChancePerAttributePoint = 2;
        public static double ReducedCellDeathPercentagePerAttributePoint = .5;
        public static int AdditionalRegrowthChancePerAttributePoint = 1;

        public static string IncreaseMutationChanceMessage =>
            $"Increase mutation chance by {AdditionalMutationPercentageChancePerAttributePoint}%.";
        public static string IncreaseCornerGrowthChanceMessage =>
            $"Increase of corner growth by {AdditionalCornerGrowthChancePerAttributePoint}%.";

        public static string DecreaseApoptosisChanceMessage =>
            $"Decrease chance of random cell death (apoptosis) by {ReducedCellDeathPercentagePerAttributePoint}%.";

        public static string IncreaseRegrowthChanceMessage => 
            $"Increase chance of reviving adjacent dead cell by {AdditionalRegrowthChancePerAttributePoint}%.";

    }
}