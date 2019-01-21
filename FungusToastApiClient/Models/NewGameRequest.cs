namespace FungusToastApiClient.Models
{
    public class NewGameRequest
    {
        public int NumberOfHumanPlayers { get; }
        public int NumberOfAiPlayers { get; }

        public NewGameRequest(int numberOfHumanPlayers, int numberOfAiPlayers = 0)
        {
            NumberOfHumanPlayers = numberOfHumanPlayers;
            NumberOfAiPlayers = numberOfAiPlayers;
        }

    }
}