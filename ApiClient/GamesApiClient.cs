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
            return MakeMockGameModelForTesting();

            //using (var client = new HttpClient())
            //{
            //    using (var response = await client.GetAsync(baseApiUrl + "/games/" + gameId))
            //    {
            //        if (response.IsSuccessStatusCode)
            //        {
            //            using (var content = response.Content)
            //            {
            //                var data = await content.ReadAsStringAsync();
            //                if (data != null)
            //                {
            //                    return _serialization.DeserializeObject<MakeMockGameModelForTesting>(data);
            //                }
            //            }
            //        }
            //    }
            //}

            throw new GameNotFoundException(gameId);
        }

        private static GameModel MakeMockGameModelForTesting()
        {
            string player1Id = "player 1 id";
            string player2Id = "player 2 id";
            return new GameModel
            {
                GenerationNumber = 1,
                Id = 2391,
                RoundNumber = 1,
                NumberOfAiPlayers = 1,
                NumberOfHumanPlayers = 2,
                NumberOfColumns = 50,
                NumberOfRows = 50,
                Status = "Not Started",
                Players = new List<PlayerState>
                {
                    new PlayerState
                    {
                        Id = player1Id,
                        Name = "jake",
                        Status = "Joined",
                        Human = true
                    },
                    new PlayerState
                    {
                        Id = player2Id,
                        Name = "Moldyman",
                        Status = "Not Joined",
                        Human = true,
                    },
                    new PlayerState
                    {
                        Id = "player id 3",
                        Name = "AI 1",
                        Status = "Not Joined",
                        Human = false,
                    }
                }
            };
        }

        public virtual async Task<GameModel> CreateGame(NewGameRequest newGame, string baseApiUrl)
        {
            return MakeMockGameModelForTesting();
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
