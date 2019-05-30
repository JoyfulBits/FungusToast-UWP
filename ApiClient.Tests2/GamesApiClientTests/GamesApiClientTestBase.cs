using System.Threading.Tasks;
using ApiClient.Models;
using ApiClient.Serialization;
using NUnit.Framework;

namespace ApiClient.Tests.GamesApiClientTests
{
    public class GamesApiClientTestBase
    {
        protected GamesApiClient GamesClient;
        protected string TestUserName = "Human 1";
        protected string TestUserName2 = "Human 2";

        [SetUp]
        public void SetUp()
        {
            GamesClient = new GamesApiClient(new Serializer());
        }

        protected async Task<GameModel> CreateValidGameForTesting(string userName, int numberOfHumanPlayers = 2, int numberOfAiPlayers = 1)
        {
            var newGameRequest = new NewGameRequest(userName, numberOfHumanPlayers, numberOfAiPlayers);
            return await GamesClient.CreateGame(newGameRequest ,TestEnvironmentSettings.BaseApiUrl);
        }
    }
}