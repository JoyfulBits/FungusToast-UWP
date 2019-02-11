using System.Threading.Tasks;
using ApiClient.Models;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture()]
    public class CreateGameTests : GamesApiClientTestBase
    {
        [Test]
        public async Task It_Creates_The_Game()
        {
            //--arrange
            var numberOfHumanPlayers = 2;
            var newGame = new NewGameRequest(TestUserName, numberOfHumanPlayers);

            //--act
            var result = await GamesClient.CreateGame(newGame, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
            result.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
            result.NumberOfRows.ShouldBe(50);
            result.NumberOfColumns.ShouldBe(50);
            result.Id.ShouldBeGreaterThan(0);
            result.Status.ShouldBe("Not Started");

            result.GenerationNumber.ShouldBe(0);
            result.RoundNumber.ShouldBe(0);
            result.TotalDeadCells.ShouldBe(0);
            result.TotalRegeneratedCells.ShouldBe(0);
            result.TotalLiveCells.ShouldBe(0);
            
            result.GrowthCycles.ShouldNotBeNull();
            //--there should be no growth cycles if the game hasn't started
            result.GrowthCycles.Count.ShouldBe(0);
            result.PreviousGameState.FungalCells.Count.ShouldBe(0);
            result.PreviousGameState.GenerationNumber.ShouldBe(0);
            result.PreviousGameState.RoundNumber.ShouldBe(0);
        }

        [Test]
        public async Task It_Automatically_Starts_The_Game_If_There_Is_Only_One_Human_Player()
        {
            //--arrange
            var numberOfHumanPlayers = 1;
            var numberOfAiPlayers = 1;
            var newGame = new NewGameRequest(TestUserName, numberOfHumanPlayers, numberOfAiPlayers);

            //--act
            var result = await GamesClient.CreateGame(newGame, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
            result.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
            result.NumberOfRows.ShouldBe(50);
            result.NumberOfColumns.ShouldBe(50);
            result.Id.ShouldBeGreaterThan(0);
            result.Status.ShouldBe("Started");

            //--if the game is started then we will start at Generation 1 and Round 1 (since live cells exist on the board for each player)
            result.GenerationNumber.ShouldBe(1);
            result.RoundNumber.ShouldBe(1);
            result.TotalDeadCells.ShouldBe(0);
            result.TotalRegeneratedCells.ShouldBe(0);
            //--we start with 1 live cell per player
            result.TotalLiveCells.ShouldBe(numberOfHumanPlayers);

            //--there is no previous game state since we start with an empty toast
            result.PreviousGameState.ShouldBeNull();
            
            result.GrowthCycles.ShouldNotBeNull();
            //--there should be a single growth cycle that places each player's starting cell
            result.GrowthCycles.Count.ShouldBe(1);
            var growthCycle = result.GrowthCycles[0];
            //--every player will start with some mutation points to spend at the start of the game
            growthCycle.MutationPointsEarned.Count.ShouldBe(2);
            foreach (var keyValuePair in growthCycle.MutationPointsEarned)
            {
                keyValuePair.Value.ShouldBeGreaterThan(0);
            }

            //--there should be one toast change (i.e. new live cell placed) for each player
            growthCycle.ToastChanges.Count.ShouldBe(numberOfHumanPlayers);
            var maxCellIndex = result.NumberOfCells - 1;
            AssertToastChangeIsCorrect(growthCycle.ToastChanges[0], maxCellIndex);
            AssertToastChangeIsCorrect(growthCycle.ToastChanges[1], maxCellIndex);

            result.Players.ShouldNotBeNull();
            result.Players.Count.ShouldBe(numberOfHumanPlayers);
            var player = result.Players[0];
            AssertPlayerLooksRight(player, true);
            player.Name.ShouldBe(TestUserName);
            player.Status.ShouldBe("Joined");

            player = result.Players[1];
            AssertPlayerLooksRight(player, false);
            player.Name.ShouldBeNull();
            player.Status.ShouldBe("Not Joined");
        }
        private static void AssertPlayerLooksRight(PlayerState player, bool shouldBeHuman)
        {
            player.Id.ShouldNotBeNullOrEmpty();
            player.Human.ShouldBe(shouldBeHuman);

            //--we will start the game with some mutation points
            player.MutationPoints.ShouldBeGreaterThan(0);

            //--you can only grow up/down/left/right to begin
            player.TopLeftGrowthChance.ShouldBe(0);
            player.TopGrowthChance.ShouldBeGreaterThan(0);
            player.TopRightGrowthChance.ShouldBe(0);
            player.RightGrowthChance.ShouldBeGreaterThan(0);
            player.BottomRightGrowthChance.ShouldBe(0);
            player.BottomGrowthChance.ShouldBeGreaterThan(0);
            player.BottomLeftGrowthChance.ShouldBe(0);
            player.LeftGrowthChance.ShouldBeGreaterThan(0);

            //--all skills start at level 0
            player.AntiApoptosisSkillLevel.ShouldBe(0);
            player.BuddingSkillLevel.ShouldBe(0);
            player.HyperMutationSkillLevel.ShouldBe(0);
            player.RegenerationSkillLevel.ShouldBe(0);
            player.MycotoxinsSkillLevel.ShouldBe(0);

            //--players' cells will start off with some positive chance of apoptosis
            player.ApoptosisChancePercentage.ShouldBeGreaterThan(0);

            //--player's cells with start off with some positive chance of death due to starvation (i.e. being surrounded by other live cells)
            player.StarvedCellDeathChancePercentage.ShouldBeGreaterThan(0);

            player.MycotoxinFungicideChancePercentage.ShouldBe(0);
            player.RegenerationChancePercentage.ShouldBe(0);

            player.MutationChancePercentage.ShouldBeGreaterThan(0);
        }

        private static void AssertToastChangeIsCorrect(ToastChange toastChange1, int maxCellIndex)
        {
            toastChange1.CellIndex.ShouldBeInRange(0, maxCellIndex);
            toastChange1.Dead.ShouldBe(false);
            toastChange1.PlayerId.ShouldNotBeNullOrEmpty();
            //--this is only set if the cell is regenerated
            toastChange1.PreviousPlayerId.ShouldBeNull();
        }
    }
}
