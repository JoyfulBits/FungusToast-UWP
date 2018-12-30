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
            $"Increase mutation chance for each generation by an additional {AdditionalMutationPercentageChancePerAttributePoint}%.";
        public static string IncreaseCornerGrowthChanceMessage =>
            $"Increase chance of a cell splitting into a diagonal corner by {AdditionalCornerGrowthChancePerAttributePoint}%.";

        public static string DecreaseCellDeathChanceMessage =>
            $"Decrease the chance of a healthy cell dying randomly by {ReducedCellDeathPercentagePerAttributePoint}%.";

        public static string IncreaseRegrowthChanceMessage => 
            $"Increase the chance of converting any adjacent dead cell into a live one by {AdditionalRegrowthChancePerAttributePoint}%.";

    }
}