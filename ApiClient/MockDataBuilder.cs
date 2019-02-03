using System;
using System.Collections.Generic;
using ApiClient.Models;

namespace ApiClient
{
    public class MockDataBuilder
    {
        public static readonly string AppUserName = "Jake";

        public static readonly string Player1Id = "Player 1 id";
        public static readonly string Player2Id = "Player 2 id";
        public static readonly string Player3Id = "AI Player 3 id";

        public static readonly string Player1Name = "Jake";
        public static readonly string Player2Name = "Other Human";
        public static readonly string Player3Name = "AI Player";

        public static GameModel MakeMockGameModelForNotStartedGame()
        {
            return new GameModel
            {
                Id = 2391,
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
                        Id = Player1Id,
                        Name = AppUserName,
                        Status = "Joined",
                        Human = true
                    },
                    new PlayerState
                    {
                        Id = Player3Id,
                        Name = Player3Name,
                        Status = "Joined",
                        Human = false,
                    }
                }
            };
        }

        public static GameModel MakeMockGameModelForJustStartedGame()
        {
            var gameModel = MakeMockGameModelForNotStartedGame();
            gameModel.Status = "Started";
            gameModel.TotalLiveCells = 3;
            gameModel.TotalEmptyCells = 2497;
            gameModel.RoundNumber = 1;
            gameModel.GenerationNumber = 1;
            gameModel.PreviousGameState.RoundNumber = 0;
            gameModel.PreviousGameState.GenerationNumber = 0;
            gameModel.Players = new List<PlayerState>
            {
                new PlayerState
                {
                    Id = Player1Id,
                    MutationPoints = 5,
                    Name = Player1Name,
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
                    Id = Player2Id,
                    MutationPoints = 0,
                    Name = Player2Name,
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
                    Id = Player3Id,
                    Name = Player3Name,
                    Status = "Joined",
                    Human = false,
                    MutationPoints = 0,
                    ApoptosisChancePercentage = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChancePercentage = 10,
                    LiveCells = 1,
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
                    GenerationNumber = 1,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 5 },
                        { Player2Id, 5 },
                        { Player3Id, 5 }
                    },
                    ToastChanges = new List<ToastChange>
                    {
                        new ToastChange
                        {
                            PlayerId = Player1Id,
                            CellIndex = 215,
                            Dead = false,
                            PreviousPlayerId = null
                        },
                        new ToastChange
                        {
                            PlayerId = Player2Id,
                            CellIndex = 1198,
                            Dead = false,
                            PreviousPlayerId = null
                        }
                    }
                }
            };

            return gameModel;
        }


        public static GameModel MakeMockGameModelForGameThatIsWellUnderWay()
        {
            var gameModel = MakeMockGameModelForNotStartedGame();
            gameModel.Status = "Started";
            gameModel.TotalDeadCells = 123;
            gameModel.TotalRegeneratedCells = 77;
            gameModel.TotalLiveCells = 400;
            gameModel.TotalEmptyCells = 1900;
            gameModel.PreviousGameState.RoundNumber = 18;
            gameModel.RoundNumber = 19;
            gameModel.PreviousGameState.GenerationNumber = 91;
            gameModel.GenerationNumber = 95;

            

            var numberOfCells = _random.Next(10, 100);
            for (int i = 0; i < numberOfCells; i++)
            {
                var keyValuePair = MakeRandomCell();
                if (!gameModel.PreviousGameState.Cells.ContainsKey(keyValuePair.Key))
                {
                    gameModel.PreviousGameState.Cells.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            gameModel.Players = new List<PlayerState>
            {
                new PlayerState
                {
                    Id = Player1Id,
                    MutationPoints = 12,
                    Name = Player1Name,
                    ApoptosisChancePercentage = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChancePercentage = 10,
                    Status = "Joined",
                    LiveCells = 200,
                    DeadCells = 100,
                    RegeneratedCells = 20,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChancePercentage = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = Player2Id,
                    MutationPoints = 0,
                    Name = Player2Name,
                    ApoptosisChancePercentage = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChancePercentage = 10,
                    Status = "Joined",
                    LiveCells = 200,
                    DeadCells = 23,
                    RegeneratedCells = 3,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChancePercentage = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = Player3Id,
                    MutationPoints = 0,
                    Name = Player3Name,
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
                    GenerationNumber = 1,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 0 },
                        { Player2Id, 1 },
                        { Player3Id, 0 }
                    },
                    ToastChanges = new List<ToastChange>
                        {
                            new ToastChange {
                                PlayerId = null,
                                CellIndex = 200,
                                Dead = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 201,
                                Dead = false,
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 202,
                                Dead = false,
                            }
                        }
                },
                new GrowthCycle
                {
                    GenerationNumber = 2,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 1 },
                        { Player2Id, 1 },
                        { Player3Id, 1 }
                    },
                    ToastChanges = new List<ToastChange>
                    {
                        new ToastChange {
                            PlayerId = null,
                            CellIndex = 203,
                            Dead = true,
                            PreviousPlayerId = Player2Id
                        },
                        new ToastChange {
                            PlayerId = Player1Id,
                            CellIndex = 204,
                            Dead = false,
                        },
                        new ToastChange {
                            PlayerId = Player2Id,
                            CellIndex = 205,
                            Dead = false,
                        }
                    }
                },
                new GrowthCycle
                {
                    GenerationNumber = 3,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 2 },
                        { Player2Id, 1 },
                        { Player3Id, 1 }
                    },
                    ToastChanges = new List<ToastChange>
                    {
                        new ToastChange {
                            PlayerId = Player1Id,
                            CellIndex = 206,
                            Dead = false,
                        },
                        new ToastChange {
                            PlayerId = Player1Id,
                            CellIndex = 207,
                            Dead = false,
                        },
                        new ToastChange {
                            PlayerId = Player3Id,
                            CellIndex = 208,
                            Dead = false,
                        }
                    }
                },
                new GrowthCycle
                {
                    GenerationNumber = 4,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 0 },
                        { Player2Id, 0 },
                        { Player3Id, 0 }
                    },
                    ToastChanges = new List<ToastChange>
                    {
                        new ToastChange {
                            PlayerId = Player1Id,
                            CellIndex = 209,
                            Dead = false,
                        },
                        new ToastChange {
                            PlayerId = Player2Id,
                            CellIndex = 210,
                            Dead = false,
                        },
                        new ToastChange {
                            PlayerId = Player3Id,
                            CellIndex = 211,
                            Dead = false
                        }
                    }
                },
                new GrowthCycle
                {
                    GenerationNumber = 5,
                    MutationPointsEarned = new Dictionary<string, int>
                    {
                        { Player1Id, 1 },
                        { Player2Id, 0 },
                        { Player3Id, 2 }
                    },
                    ToastChanges = new List<ToastChange>
                        {
                            new ToastChange
                            {
                                PlayerId = Player1Id,
                                CellIndex = 1,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = null,
                                CellIndex = 2,
                                Dead = true,
                                PreviousPlayerId = Player1Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 6,
                                Dead = false,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 8,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 23,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 26,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 27,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 123,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 1366,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                CellIndex = 1456,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1457,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1458,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1460,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1461,
                                Dead = false,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1469,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1653,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                CellIndex = 1655,
                                Dead = false,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = null,
                                CellIndex = 1656,
                                Dead = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = null,
                                CellIndex = 2011,
                                Dead = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player3Id,
                                CellIndex = 2012,
                                Dead = false,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player3Id,
                                CellIndex = 2012,
                                Dead = false,
                                PreviousPlayerId = null
                            }
                        }
                }
            };

            return gameModel;
        }

        private static readonly Random _random = new Random();

        private static KeyValuePair<int, FungalCell> MakeRandomCell()
        {
            var cellIndex = _random.Next(0, 2499);

            var playerIndex = _random.Next(0, 3);

            string playerId = Player1Id;
            switch (playerIndex)
            {
                case 0:
                    playerId = Player1Id;
                    break;
                case 1:
                    playerId = Player2Id;
                    break;
                case 2:
                    playerId = Player3Id;
                    break;
            }

            //--make 1 in 3 cells dead
            bool dead = playerIndex % 3 == 0;
            FungalCell fungalCell;
            if (dead)
            {
                fungalCell = new FungalCell
                {
                    PreviousPlayerId = playerId,
                    Dead = true
                };
            }
            else
            {
                fungalCell = new FungalCell
                {
                    PlayerId = playerId,
                };
            }

            return new KeyValuePair<int, FungalCell>(cellIndex, fungalCell);
        }
    }
}
