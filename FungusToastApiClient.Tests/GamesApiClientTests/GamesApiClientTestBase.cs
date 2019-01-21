using System.Threading.Tasks;
using FungusToastApiClient.Models;
using NUnit.Framework;

namespace FungusToastApiClient.Tests.GamesApiClientTests
{
    public class GamesApiClientTestBase
    {
        protected GamesApiClient GamesClient;

        [SetUp]
        public void SetUp()
        {
            GamesClient = new GamesApiClient();
        }

        protected async Task<GameState> CreateValidGameForTesting(int numberOfHumanPlayers = 2, int numberOfAiPlayers = 1)
        {
            var newGameRequest = new NewGameRequest(numberOfHumanPlayers, numberOfAiPlayers);
            return await GamesClient.CreateGame(newGameRequest ,TestEnvironmentSettings.BaseApiUrl);
        }
    }
}