namespace ApiClient.Models
{
    public class FungalCell
    {
        public int Index { get; set; }
        public string PlayerId { get; set; }
        public bool Live { get; set; }
        public string PreviousPlayerId { get; set; }
    }
}