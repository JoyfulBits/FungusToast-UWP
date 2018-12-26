using System.Collections.Generic;

namespace Logic
{
    public class MutationOptionGenerator : IMutationOptionGenerator
    {
        public static int AdditionalMutationPercentageChancePerAttributePoint = 3;
        public static int AdditionalCornerGrowthChancePerAttributePoint = 2;
        public static int ReducedCellDeathPercentagePerAttributePoint = 1;

        public MutationChoice GetMutationChoices(IPlayer activePlayer, List<IPlayer> allPlayers)
        {
            return new MutationChoice
            {
                IncreaseMutationChance = true,
                IncreaseCornerGrowthChance = true,
                DecreaseHealthyCellDeathChance = true
            };
        }

        public static string IncreaseMutationChanceMessage =>
            $"Increase mutation chance for each generation by an additional {AdditionalMutationPercentageChancePerAttributePoint}%.";
        public static string IncreaseCornerGrowthChanceMessage =>
            $"Increase chance of a cell splitting into a diagonal corner by {AdditionalCornerGrowthChancePerAttributePoint}%.";

        public static string DecreaseCellDeathChanceMessage =>
            $"Decrease the chance of a healthy cell dying randomly by {ReducedCellDeathPercentagePerAttributePoint}%.";
    }
}