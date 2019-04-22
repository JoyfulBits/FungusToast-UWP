using System;
using System.Collections.Generic;
using ApiClient.Models;

namespace ApiClient
{
    public class MockDataBuilder
    {
        public static readonly string AppUserName = "jakejgordon";

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
                GridSize = 50,
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
            gameModel.Players = new List<PlayerState>
            {
                new PlayerState
                {
                    Id = Player1Id,
                    MutationPoints = 5,
                    Name = Player1Name,
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    Status = "Joined",
                    LiveCells = 1,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = Player2Id,
                    MutationPoints = 0,
                    Name = Player2Name,
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    Status = "Joined",
                    LiveCells = 1,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
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
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    LiveCells = 1,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
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
                            Index = 215,
                            Live = true,
                            PreviousPlayerId = null
                        },
                        new ToastChange
                        {
                            PlayerId = Player2Id,
                            Index = 1198,
                            Live = true,
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
            gameModel.RoundNumber = 19;
            gameModel.GenerationNumber = 95;

            var numberOfCells = _random.Next(10, 100);
            var keysAlreadyAdded = new HashSet<int>();
            for (var i = 0; i < numberOfCells; i++)
            {
                var fungalCell = MakeRandomCell();

                if (!keysAlreadyAdded.Contains(fungalCell.Index))
                {
                    keysAlreadyAdded.Add(fungalCell.Index);
                    gameModel.StartingGameState.FungalCells.Add(fungalCell);
                }
            }

            gameModel.Players = new List<PlayerState>
            {
                new PlayerState
                {
                    Id = Player1Id,
                    MutationPoints = 12,
                    Name = Player1Name,
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    Status = "Joined",
                    LiveCells = 200,
                    DeadCells = 100,
                    RegeneratedCells = 20,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = Player2Id,
                    MutationPoints = 0,
                    Name = Player2Name,
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    Status = "Joined",
                    LiveCells = 200,
                    DeadCells = 23,
                    RegeneratedCells = 3,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
                    AntiApoptosisSkillLevel = 7,
                    LeftGrowthChance = 7.5,
                    TopGrowthChance = 7.5
                },
                new PlayerState
                {
                    Id = Player3Id,
                    MutationPoints = 0,
                    Name = Player3Name,
                    ApoptosisChance = 5,
                    RightGrowthChance = 7.5,
                    StarvedCellDeathChance = 10,
                    Status = "Joined",
                    LiveCells = 1,
                    Human = true,
                    BottomGrowthChance = 7.5,
                    MutationChance = 10,
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
                                Index = 200,
                                Live = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 201,
                                Live = true,
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                Index = 202,
                                Live = true,
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
                            Index = 203,
                            Live = true,
                            PreviousPlayerId = Player2Id
                        },
                        new ToastChange {
                            PlayerId = Player1Id,
                            Index = 204,
                            Live = true,
                        },
                        new ToastChange {
                            PlayerId = Player2Id,
                            Index = 205,
                            Live = true,
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
                            Index = 206,
                            Live = true,
                        },
                        new ToastChange {
                            PlayerId = Player1Id,
                            Index = 207,
                            Live = true,
                        },
                        new ToastChange {
                            PlayerId = Player3Id,
                            Index = 208,
                            Live = true,
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
                            Index = 209,
                            Live = true,
                        },
                        new ToastChange {
                            PlayerId = Player2Id,
                            Index = 210,
                            Live = true,
                        },
                        new ToastChange {
                            PlayerId = Player3Id,
                            Index = 211,
                            Live = true
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
                                Index = 1,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = null,
                                Index = 2,
                                Live = true,
                                PreviousPlayerId = Player1Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 6,
                                Live = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 8,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 23,
                                Live = true,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player1Id,
                                Index = 26,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 27,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 123,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 1366,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player1Id,
                                Index = 1456,
                                Live = true,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1457,
                                Live = true,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1458,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1460,
                                Live = true,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1461,
                                Live = true,
                                PreviousPlayerId = null
                            },new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1469,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1653,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = Player2Id,
                                Index = 1655,
                                Live = true,
                                PreviousPlayerId = null
                            },
                            new ToastChange {
                                PlayerId = null,
                                Index = 1656,
                                Live = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = null,
                                Index = 2011,
                                Live = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player3Id,
                                Index = 2012,
                                Live = true,
                                PreviousPlayerId = Player2Id
                            },
                            new ToastChange {
                                PlayerId = Player3Id,
                                Index = 2012,
                                Live = true,
                                PreviousPlayerId = null
                            }
                        }
                }
            };

            return gameModel;
        }

        private static readonly Random _random = new Random();

        private static FungalCell MakeRandomCell()
        {
            var cellIndex = _random.Next(0, 2499);

            var playerIndex = _random.Next(0, 3);

            var playerId = Player1Id;
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
            var dead = playerIndex % 3 == 0;
            var fungalCell = new FungalCell
            {
                Index = cellIndex
            };

            if (dead)
            {
                fungalCell.PreviousPlayerId = playerId;
                fungalCell.Live = false;
            }
            else
            {
                fungalCell.PlayerId = playerId;
            }

            return fungalCell;
        }
    }
}
