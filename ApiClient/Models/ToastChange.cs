namespace ApiClient.Models
{
    public class ToastChange
    {
        public string PreviousPlayerId;
        public int CellIndex { get; set; }
        public string PlayerId { get; set; }
        public bool Dead { get; set; }
    }
}