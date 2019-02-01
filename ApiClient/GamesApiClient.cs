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
            return MakeMockGameModelForJustStartedGame();

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
            //                    return _serialization.DeserializeObject<MakeMockGameModelForNotStartedGame>(data);
            //                }
            //            }
            //        }
            //    }
            //}

            throw new GameNotFoundException(gameId);
        }

        public virtual async Task<GameModel> CreateGame(NewGameRequest newGame, string baseApiUrl)
        {
            return MakeMockGameModelForNotStartedGame();
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

        private static GameModel MakeMockGameModelForNotStartedGame()
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
                JoinGamePassword = "password",
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
                        Id = "player id 3",
                        Name = "AI 1",
                        Status = "Not Joined",
                        Human = false,
                    }
                }
            };
        }

        private static GameModel MakeMockGameModelForJustStartedGame()
        {
            var player1Id = "player 1 id";
            var player2Id = "player 2 id";
            var gameModel = MakeMockGameModelForNotStartedGame();
            gameModel.Status = "Started";
            gameModel.Players = new List<PlayerState>
            {
                new PlayerState
                {
                    Id = player1Id,
                    MutationPoints = 5,
                    Name = "jake",
                    ApoptosisChancePercentage = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChancePercentage = 10,
                    Status = "Joined",
                    LiveCells = 1,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChancePercentage = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = player2Id,
                    MutationPoints = 0,
                    Name = "player 2 name",
                    ApoptosisChancePercentage = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChancePercentage = 10,
                    Status = "Joined",
                    LiveCells = 1,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChancePercentage = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                }
            };
            gameModel.GrowthCycles = new List<GrowthCycle>
            {
                new GrowthCycle
                {
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { player1Id, 5 },
                        { player2Id, 5 }
                    },
                    ToastChanges = new List<ToastChange>
                    {
                        new ToastChange
                        {
                            PlayerId = player1Id,
                            CellIndex = 215,
                            Dead = false,
                            PreviousPlayerId = null
                        },
                        new ToastChange
                        {
                            PlayerId = player2Id,
                            CellIndex = 1198,
                            Dead = false,
                            PreviousPlayerId = null
                        }
                    }
                }
            };

            return gameModel;
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
