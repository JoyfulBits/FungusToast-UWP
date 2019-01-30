using System.Collections.Generic;
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

        public async Task<GameModel> GetGameState(int gameId)
        {
            return await _gamesApiClient.GetGameState(gameId, _baseApiUrl);
        }

        public async Task<GameModel> CreateGame(NewGameRequest newGame)
        {
            return await _gamesApiClient.CreateGame(newGame, _baseApiUrl);
        }

        public async Task<SkillUpdateResult> PushSkillExpenditures(SkillExpenditureRequest skillExpenditureRequest)
        {
            return await _gamesApiClient.PushSkillExpenditures(skillExpenditureRequest, _baseApiUrl);
        }
    }
}