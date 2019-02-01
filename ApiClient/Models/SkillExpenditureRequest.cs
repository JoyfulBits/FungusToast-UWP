using System.Collections.Generic;

namespace ApiClient.Models
{
    public class SkillExpenditureRequest
    {
        public int GameId { get; }
        public string PlayerId { get; }
        public SkillExpenditure SkillExpenditure { get; }

        public SkillExpenditureRequest(int gameId, string playerId, SkillExpenditure skillExpenditure)
        {
            GameId = gameId;
            PlayerId = playerId;
            SkillExpenditure = skillExpenditure;
        }
    }
}
