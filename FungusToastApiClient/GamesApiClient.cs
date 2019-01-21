﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using FungusToastApiClient.Exceptions;
using FungusToastApiClient.Models;
using FungusToastApiClient.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FungusToastApiClient
{
    public class GamesApiClient
    {
        private readonly ISerializer _serialization;

        public GamesApiClient(ISerializer serialization)
        {
            _serialization = serialization;
        }

        public async Task<GameState> GetGameState(int gameId, string baseApiUrl)
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
                                return _serialization.DeserializeObject<GameState>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotFoundException(gameId);
        }

        public async Task<GameState> CreateGame(NewGameRequest newGame, string baseApiUrl)
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
                                return _serialization.DeserializeObject<GameState>(data);
                            }
                        }
                    }
                }
            }

            throw new GameNotCreatedException(newGame);
        }
    }
}
