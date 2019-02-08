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

        public virtual async Task<GameModel> GetGameState(int gameId, string baseApiUrl, MockOption? mockOption = null)
        {
            if (mockOption.HasValue)
            {
                if (mockOption.Value == MockOption.NewGame)
                {
                    return MockDataBuilder.MakeMockGameModelForJustStartedGame();
                }

                return MockDataBuilder.MakeMockGameModelForGameThatIsWellUnderWay();
            }

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

        public virtual async Task<GameModel> CreateGame(NewGameRequest newGame, string baseApiUrl, bool returnMock = false)
        {
            if (returnMock)
            {
                return MockDataBuilder.MakeMockGameModelForJustStartedGame();
            }

            using (var client = new HttpClient())
            {
                var stringifiedObject = _serialization.SerializeToHttpStringContent(newGame);
               
                var gamesUri = new Uri(baseApiUrl + "/games");
                using (var response = await client.PostAsync(gamesUri, stringifiedObject))
                {
                    using (var content = response.Content)
                    {
                        var data = await content.ReadAsStringAsync();
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
            SkillExpenditureRequest skillExpenditureRequest, string baseApiUrl, bool? mockNextRoundAvailable = null)
        {
            if (mockNextRoundAvailable.HasValue)
            {
                return new SkillUpdateResult
                {
                    NextRoundAvailable = mockNextRoundAvailable.Value
                };
            }

            using (var client = new HttpClient())
            {
                var stringifiedObject = _serialization.SerializeToHttpStringContent(skillExpenditureRequest);

                var uri = new Uri(baseApiUrl + $"/games/{gameId}/players/{playerId}/skills");
                using (var response = await client.PostAsync(uri, stringifiedObject))
                {
                    using (var content = response.Content)
                    {
                        var data = await content.ReadAsStringAsync();
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
    }
}
