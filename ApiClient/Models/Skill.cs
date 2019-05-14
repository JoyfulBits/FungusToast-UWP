namespace ApiClient.Models
{
    public class Skill
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public float IncreasePerPoint { get; set; }
        public bool UpIsGood { get; set; }
    }
}
