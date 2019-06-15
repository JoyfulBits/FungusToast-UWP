namespace ApiClient.Models
{
    public class PassiveSkill
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public float IncreasePerPoint { get; set; }
        public bool UpIsGood { get; set; }
        public int MinimumRound { get; set; }
    }
}
