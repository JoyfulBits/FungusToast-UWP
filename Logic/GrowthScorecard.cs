using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class GrowthScorecard
    {
        public const int BaseGrowthPercentage = 10;

        public Dictionary<RelativePosition, int> GrowthChanceDictionary = new Dictionary<RelativePosition, int>
        {
            { RelativePosition.TopLeft, 0 },
            { RelativePosition.Top, BaseGrowthPercentage },
            { RelativePosition.TopRight, 0 },
            { RelativePosition.Right, BaseGrowthPercentage },
            { RelativePosition.BottomRight, 0 },
            { RelativePosition.Bottom, BaseGrowthPercentage },
            { RelativePosition.BottomLeft, 0 },
            { RelativePosition.Left, BaseGrowthPercentage }
        };

        public int GetGrowthChance(RelativePosition position)
        {
            return GrowthChanceDictionary[position];
        }
    }
}
