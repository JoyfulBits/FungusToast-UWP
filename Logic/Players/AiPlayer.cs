using System;
using Windows.UI;

namespace Logic.Players
{
    [Obsolete("AI logic will be handled server-side")]
    public class AiPlayer : Player, IAiPlayer
    {
        public AiPlayer(string name, Color playerCellColor, int playerId,
            ICellGrowthCalculator cellGrowthCalculator, ISurroundingCellCalculator surroundingCellCalculator,
            bool isHuman, AiType? aiType = null) :
            base(name, playerCellColor, playerId, cellGrowthCalculator, surroundingCellCalculator, isHuman)
        {
            if (aiType == null)
            {
                var numberOfMembers = Enum.GetNames(typeof(AiType)).Length;
                var aiTypeIndex = RandomNumberGenerator.Random.Next(0, numberOfMembers - 1);
                AiType = (AiType)aiTypeIndex;
            }
            else
            {
                AiType = aiType.Value;
            }
    }

        public AiType AiType { get;  }
    }
}
