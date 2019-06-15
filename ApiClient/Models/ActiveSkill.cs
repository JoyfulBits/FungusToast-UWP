namespace ApiClient.Models
{
    public class ActiveSkill
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfToastChanges { get; set; }
        public int MinimumRound { get; set; }
    }
}
