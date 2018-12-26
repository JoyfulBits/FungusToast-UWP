using System.Collections.Generic;

namespace Logic
{
    public interface IMutationOptionGenerator
    {
        MutationChoice GetMutationChoices(IPlayer activePlayer, List<IPlayer> allPlayers);
    }
}