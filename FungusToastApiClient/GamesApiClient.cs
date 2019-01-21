using System;
using System.Net.Http;
using System.Threading.Tasks;
using FungusToastApiClient.Exceptions;
using FungusToastApiClient.Models;
using Newtonsoft.Json;

namespace FungusToastApiClient
{
    public class GamesApiClient
    {
        public async Task<GameState> GetGameState(int gameId, string baseApiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(baseApiUrl + "/games/" + gameId))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return JsonConvert.DeserializeObject<GameState>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotFoundException(gameId);
        }

        public async Task<GameState> CreateGame(NewGameRequest newGame, string baseApiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonObject = JsonConvert.SerializeObject(newGame);
                var stringifiedObject = new StringContent(jsonObject);
                var gamesUri = new Uri(baseApiUrl + "/games");
                using (HttpResponseMessage response = await client.PostAsync(gamesUri, stringifiedObject))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return JsonConvert.DeserializeObject<GameState>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotCreatedException(newGame);
        }
    }
}
