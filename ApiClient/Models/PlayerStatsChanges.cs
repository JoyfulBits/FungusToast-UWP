namespace ApiClient.Models
{
    public class PlayerStatsChanges
    {
        public int GrownCells { get; set; }
        public int PerishedCells { get; set; }
        public int RegeneratedCells { get; set; }
        public int StolenDeadCells { get; set; }
        public int LostDeadCells { get; set; }
        public int FungicidalKills { get; set; }
    }
}