using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var uri = new Uri(baseApiUrl + "/games/" + gameId);
                using (var response = await client.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            Debug.WriteLine($"GET request sent to '{uri.AbsolutePath}'.");
                            Debug.WriteLine($"Got response: '{data}'");

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
                    using (var content = response.Content)
                    {
                        var data = await content.ReadAsStringAsync();
                        Debug.WriteLine($"POST request sent to '{gamesUri.AbsolutePath}' with data: '{await stringifiedObject.ReadAsStringAsync()}'.");
                        Debug.WriteLine($"Got response: '{data}'");

                        if (data != null && response.IsSuccessStatusCode)
                        {
                            return _serialization.DeserializeObject<GameModel>(data);
                        }

                        var requestDataJson = await stringifiedObject.ReadAsStringAsync();
                        throw new ApiException(gamesUri, HttpMethod.Post, requestDataJson, response.StatusCode, data);
                    }
                }
            }
        }

        public virtual async Task<SkillUpdateResult> PushSkillExpenditures(int gameId, string playerId,
            SkillExpenditureRequest skillExpenditureRequest, string baseApiUrl)
        {
            using (var client = new HttpClient())
            {
                var stringifiedObject = _serialization.SerializeToHttpStringContent(skillExpenditureRequest);
                
                var uri = new Uri(baseApiUrl + $"/games/{gameId}/players/{playerId}/skills");
                using (var response = await client.PostAsync(uri, stringifiedObject))
                {
                    using (var content = response.Content)
                    {
                        var data = await content.ReadAsStringAsync();
                        Debug.WriteLine($"POST request sent to '{uri.AbsolutePath}' with data: '{await stringifiedObject.ReadAsStringAsync()}'.");
                        Debug.WriteLine($"Got response: '{data}'");

                        if (data != null && response.IsSuccessStatusCode)
                        {
                            return _serialization.DeserializeObject<SkillUpdateResult>(data);
                        }

                        var requestDataJson = await stringifiedObject.ReadAsStringAsync();
                        throw new ApiException(uri, HttpMethod.Post, requestDataJson, response.StatusCode, data);
                    }
                }
            }
        }

        public virtual async Task<List<Skill>> GetSkills(string baseApiUrl)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri(baseApiUrl + "/skills");
                using (var response = await client.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            Debug.WriteLine($"GET request sent to '{uri.AbsolutePath}'.");
                            Debug.WriteLine($"Got response: '{data}'");

                            if (data != null)
                            {
                                return _serialization.DeserializeObject<List<Skill>>(data);
                            }
                        }
                    }

                    throw new ApiException(uri, HttpMethod.Post, string.Empty, response.StatusCode, string.Empty);
                }
            }
        }
    }
}
