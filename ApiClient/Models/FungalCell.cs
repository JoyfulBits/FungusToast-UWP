namespace ApiClient.Models
{
    public class FungalCell
    {
        public int CellIndex { get; set; }
        public string PlayerId { get; set; }
        public bool Dead { get; set; }
        public string PreviousPlayerId { get; set; }
    }
}