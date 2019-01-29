using System;
using System.Net.Http;
using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;
using ApiClient.Serialization;

namespace ApiClient
{
    public class GamesApiClient
    {
        private readonly ISerializer _serialization;

        public GamesApiClient(ISerializer serialization)
        {
            _serialization = serialization;
        }

        public async Task<GameModel> GetGameState(int gameId, string baseApiUrl)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(baseApiUrl + "/games/" + gameId))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return _serialization.DeserializeObject<GameModel>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotFoundException(gameId);
        }

        public async Task<GameModel> CreateGame(NewGameRequest newGame, string baseApiUrl)
        {
            using (var client = new HttpClient())
            {
                var stringifiedObject = _serialization.SerializeToHttpStringContent(newGame);
               
                var gamesUri = new Uri(baseApiUrl + "/games");
                using (var response = await client.PostAsync(gamesUri, stringifiedObject))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return _serialization.DeserializeObject<GameModel>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotCreatedException(newGame);
        }
    }
}
