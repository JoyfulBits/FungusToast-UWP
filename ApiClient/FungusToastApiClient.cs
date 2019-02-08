using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;

namespace ApiClient
{
    public class FungusToastApiClient : IFungusToastApiClient
    {
        private readonly string _baseApiUrl;
        private readonly GamesApiClient _gamesApiClient;

        public FungusToastApiClient(string baseApiUrl, GamesApiClient gamesApiClient)
        {
            _baseApiUrl = baseApiUrl;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<GameModel> GetGameState(int gameId, MockOption? mockOption = null)
        {
            return await _gamesApiClient.GetGameState(gameId, _baseApiUrl, mockOption);
        }

        public async Task<GameModel> CreateGame(NewGameRequest newGame, bool returnMock = false)
        {
            return await _gamesApiClient.CreateGame(newGame, _baseApiUrl, returnMock);
        }

        public async Task<SkillUpdateResult> PushSkillExpenditures(int gameId, string playerId, SkillExpenditureRequest skillExpenditureRequest,
            bool? mockNextRoundAvailable = null)
        {
            return await _gamesApiClient.PushSkillExpenditures(gameId, playerId, skillExpenditureRequest, _baseApiUrl, mockNextRoundAvailable);
        }
    }
}