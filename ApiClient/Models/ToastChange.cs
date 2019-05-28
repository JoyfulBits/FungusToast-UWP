namespace ApiClient.Models
{
    public class ToastChange
    {
        public string PreviousPlayerId;
        public int Index { get; set; }
        public string PlayerId { get; set; }
        public bool Live { get; set; }
        public bool Moist { get; set; }
    }
}