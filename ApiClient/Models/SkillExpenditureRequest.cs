using System.Collections.Generic;

namespace ApiClient.Models
{
    public class SkillExpenditureRequest
    {
        public int GameId { get; }
        public string PlayerId { get; }
        public List<SkillExpenditure> SkillExpenditures { get; }

        public SkillExpenditureRequest(int gameId, string playerId, List<SkillExpenditure> skillExpenditures)
        {
            GameId = gameId;
            PlayerId = playerId;
            SkillExpenditures = skillExpenditures;
        }
    }
}
