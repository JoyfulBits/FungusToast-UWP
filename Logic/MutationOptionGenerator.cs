using System.Collections.Generic;

namespace Logic
{
    public class MutationOptionGenerator : IMutationOptionGenerator
    {
        public static int AdditionalMutationPercentageChancePerAttributePoint = 5;
        public static int AdditionalCornerGrowthChancePerAttributePoint = 2;

        public MutationChoice GetMutationChoices(IPlayer activePlayer, List<IPlayer> allPlayers)
        {
            return new MutationChoice
            {
                IncreaseMutationChance = true,
                IncreaseCornerGrowthChance = true
            };
        }

        public static string IncreaseMutationChanceMessage =>
            $"Increase mutation chance for each generation by an additional {AdditionalMutationPercentageChancePerAttributePoint}%.";
        public static string IncreaseCornerGrowthChanceMessage =>
            $"Increase chance of a cell splitting into a diagonal corner by {AdditionalCornerGrowthChancePerAttributePoint}%.";
    }
}