namespace ApiClient.Models
{
    public class ActiveCellChange
    {
        public int Index { get; }
        public int SkillId { get; }
        public string PlayerId { get; }

        public ActiveCellChange(string playerId, int index, int skillId)
        {
            PlayerId = playerId;
            Index = index;
            SkillId = skillId;
        }
    }
}
