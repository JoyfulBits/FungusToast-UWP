namespace ApiClient.Models
{
    public class GameState
    {
        public int Id { get; set; }
        public int NumberOfHumanPlayers { get; set; }
        public int NumberOfAiPlayers { get; set; }
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public bool Active { get; set; }
    }
}
