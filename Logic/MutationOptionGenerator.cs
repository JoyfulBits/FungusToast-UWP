using System.Collections.Generic;

namespace Logic
{
    public class MutationOptionGenerator : IMutationOptionGenerator
    {
        public static int AdditionalMutationPercentageChancePerAttributePoint = 5;

        public MutationChoice GetMutationChoices(IPlayer activePlayer, List<IPlayer> allPlayers)
        {
            return new MutationChoice
            {
                IncreaseMutationChance = true
            };
        }

        public static string IncreaseMutationChanceMessage =>
            $"Increase mutation chance for each generation by an additional {AdditionalMutationPercentageChancePerAttributePoint}%.";

    }
}