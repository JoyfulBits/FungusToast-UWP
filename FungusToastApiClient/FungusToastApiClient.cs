using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using FungusToastApiClient.Exceptions;
using FungusToastApiClient.Models;
using Newtonsoft.Json;

namespace FungusToastApiClient
{
    public class FungusToastApiClient
    {
        private readonly string _baseApiUrl;
        private readonly GamesApiClient _gamesApiClient;

        public FungusToastApiClient(string baseApiUrl, GamesApiClient gamesApiClient)
        {
            _baseApiUrl = baseApiUrl;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<GameState> GetGameState(int gameId)
        {
            return await _gamesApiClient.GetGameState(gameId, _baseApiUrl);
        }

        
    }
}