using System;
using System.Collections.Generic;
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

        public virtual async Task<GameModel> GetGameState(int gameId, string baseApiUrl)
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

        public virtual async Task<GameModel> CreateGame(NewGameRequest newGame, string baseApiUrl)
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

        public virtual async Task<SkillUpdateResult> PushSkillExpenditures(SkillExpenditureRequest skillExpenditureRequest, string baseApiUrl)
        {
            using (var client = new HttpClient())
            {
                var stringifiedObject = _serialization.SerializeToHttpStringContent(skillExpenditureRequest);

                var gamesUri = new Uri(baseApiUrl + "/player-skills");
                using (var response = await client.PostAsync(gamesUri, stringifiedObject))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return _serialization.DeserializeObject<SkillUpdateResult>(data);
                            }
                        }
                    }
                }
            }

            throw new SkillsNotUpdatedException(skillExpenditureRequest);
        }
    }
}
