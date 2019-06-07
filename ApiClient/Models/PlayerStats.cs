namespace ApiClient.Models
{
    public class PlayerStats
    {
        public int LiveCells { get; set; }
        public int DeadCells { get; set; }
        public int GrownCells { get; set; }
        public int PerishedCells { get; set; }
        public int RegeneratedCells { get; set; }
        public int FungicidalKills { get; set; }
        public int LostDeadCells { get; set; }
        public int StolenDeadCells { get; set; }
    }
}