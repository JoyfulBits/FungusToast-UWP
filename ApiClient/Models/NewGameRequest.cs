namespace ApiClient.Models
{
    public class NewGameRequest
    {
        public string UserName { get; }
        public int NumberOfHumanPlayers { get; }
        public int NumberOfAiPlayers { get; }

        public NewGameRequest(string userName, int numberOfHumanPlayers, int numberOfAiPlayers = 0)
        {
            UserName = userName;
            NumberOfHumanPlayers = numberOfHumanPlayers;
            NumberOfAiPlayers = numberOfAiPlayers;
        }

    }
}