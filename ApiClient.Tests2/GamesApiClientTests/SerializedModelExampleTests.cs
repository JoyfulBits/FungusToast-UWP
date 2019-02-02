using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ApiClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class SerializedModelExampleTests
    {
        [Category("Integration")]
        [Test]
        public void Example_Model_Json_For_New_Game_Not_Started()
        {
            //--arrange
            var player1Id = "player 1 id";
            var gameModel = new GameModel
            {
                GenerationNumber = 1,
                Id = 2391,
                RoundNumber = 1,
                NumberOfAiPlayers = 1,
                NumberOfHumanPlayers = 2,
                NumberOfColumns = 50,
                NumberOfRows = 50,
                Status = "Not Started",
                JoinGamePassword = "GY3LO2",
                Players = new List<PlayerState>
                {
                    new PlayerState
                    {
                        Id = player1Id,
                        Name = "Slowdawg 50",
                        Status = "Joined",
                        Human = true,
                    },
                    new PlayerState
                    {
                        Id = "player id 3",
                        Name = "Organic Mold #151621",
                        Status = "Not Joined",
                        Human = false,
                    }
                }
            };

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_Model_Json_For_New_That_Just_Started()
        {
            //--arrange
            var player1Id = "player 1 id";
            var player2Id = "player 2 id";
            var gameModel = new GameModel
            {
                GenerationNumber = 1,
                Id = 2391,
                RoundNumber = 1,
                NumberOfAiPlayers = 0,
                NumberOfHumanPlayers = 2,
                NumberOfColumns = 45,
                NumberOfRows = 45,
                Status = "Started",
                JoinGamePassword = "GY3LO2",
                TotalLiveCells = 2,
                TotalEmptyCells = 2023,
                Players = new List<PlayerState>
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
                        MutationPoints = 5,
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
                },
                GrowthCycles = new List<GrowthCycle>
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
                }
            };

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_Model_Json_For_Game_That_Is_In_Progress()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForGameThatIsWellUnderWay();

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_SkillExpenditure()
        {
            //--arrange
            var request = new SkillExpenditureRequest(1, "player id 1", new SkillExpenditure
            {
                AntiApoptosisPoints = 1,
                BuddingPoints = 2,
                HypermutationPoints = 0,
                MycotoxicityPoints = 1,
                RegenerationPoints = 3
            });

            //--act
            var jsonObject = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            //--assert
            Debug.WriteLine(jsonObject);
        }
    }
}
