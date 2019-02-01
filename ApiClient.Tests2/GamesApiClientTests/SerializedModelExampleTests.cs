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
        [Ignore("Used for providing JSON examples to API developers only")]
        [Test]
        public void Example_Model_Json_For_New_Game_Not_Started()
        {
            //--arrange
            var player1Id = "player 1 id";
            var player2Id = "player 2 id";
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
                        Id = player2Id,
                        Name = "Moldy Mouse",
                        Status = "Not Joined",
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
                TotalLiveCells = 2,
                TotalEmptyCells = 2023,
                Players = new List<PlayerState>
                {
                    new PlayerState
                    {
                        Id = player1Id,
                        MutationPoints = 5,
                        Name = "player 1 name",
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
            var player1Id = "player 1 id";
            var player2Id = "player 2 id";
            var gameModel = new GameModel
            {
                GenerationNumber = 55,
                Id = 2391,
                RoundNumber = 11,
                NumberOfAiPlayers = 0,
                NumberOfHumanPlayers = 2,
                NumberOfColumns = 45,
                NumberOfRows = 45,
                Status = "Started",
                TotalLiveCells = 525,
                TotalDeadCells = 400,
                TotalRegeneratedCells = 40,
                TotalEmptyCells = 1125,
                Players = new List<PlayerState>
                {
                    new PlayerState
                    {
                        Id = player1Id,
                        MutationPoints = 9,
                        Name = "player 1 name",
                        MycotoxinFungicideChancePercentage = 0.2,
                        MycotoxinsSkillLevel = 2,
                        BuddingSkillLevel = 7,
                        TopLeftGrowthChance = 7,
                        TopRightGrowthChance = 7,
                        BottomRightGrowthChance = 7,
                        BottomLeftGrowthChance = 7,
                        ApoptosisChancePercentage = 3.4,
                        RightGrowthChance = 7.5,
                        StarvedCellDeathChancePercentage = 10,
                        Status = "Joined",
                        LiveCells = 300,
                        Human = true,
                        BottomGrowthChance = 7.5,
                        MutationChancePercentage = 26,
                        AntiApoptosisSkillLevel = 7,
                        LeftGrowthChance = 7.5,
                        TopGrowthChance = 7.5,
                        HyperMutationSkillLevel = 8
                    },
                    new PlayerState
                    {
                        Id = player2Id,
                        MutationPoints = 6,
                        Name = "player 2 name",
                        ApoptosisChancePercentage = 5,
                        BuddingSkillLevel = 10,
                        TopLeftGrowthChance = 10,
                        TopRightGrowthChance = 10,
                        BottomRightGrowthChance = 10,
                        BottomLeftGrowthChance = 10,
                        RightGrowthChance = 7.5,
                        StarvedCellDeathChancePercentage = 10,
                        Status = "Joined",
                        LiveCells = 225,
                        Human = true,
                        BottomGrowthChance = 7.5,
                        MutationChancePercentage = 10,
                        AntiApoptosisSkillLevel = 7,
                        LeftGrowthChance = 7.5,
                        TopGrowthChance = 7.5,
                        HyperMutationSkillLevel = 3
                    }
                },
                GrowthCycles = new List<GrowthCycle>
                {
                    new GrowthCycle
                    {
                        MutationPointsEarned = new Dictionary<string, int>
                        {
                            { player1Id, 9 },
                            { player2Id, 6 }
                        },
                        ToastChanges = new List<ToastChange>
                        {
                            new ToastChange
                            {
                            PlayerId = player1Id,
                            CellIndex = 1,
                            Dead = false,
                            PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 2,
                                Dead = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 6,
                                Dead = false,
                                PreviousPlayerId = player2Id
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 8,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 23,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 26,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 27,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 123,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 1366,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player1Id,
                                CellIndex = 1456,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1457,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1458,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1460,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1461,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1469,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1653,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1655,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 1656,
                                Dead = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = player2Id,
                                CellIndex = 2011,
                                Dead = true,
                                PreviousPlayerId = null
                            }
                        }
                    }
                },
                PreviousGameState = new GameState
                {
                    Cells = new Dictionary<int, FungalCell>
                    {
                        { 0, new FungalCell
                            {
                                PlayerId = player1Id
                            }}
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
    }
}
