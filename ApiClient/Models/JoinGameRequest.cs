namespace ApiClient.Models
{
    public class JoinGameRequest
    {
        public int GameId { get; }
        public string UserName { get; }

        public JoinGameRequest(int gameId, string userName)
        {
            GameId = gameId;
            UserName = userName;
        }
    }
}
