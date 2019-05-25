using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class GetGameStateTests : GamesApiClientTestBase
    {
        [Test]
        public void It_Throws_A_GameStateNotFoundException_If_It_Cant_Find_The_Game()
        {
            //--arrange
            var invalidGameID = -1;
            var expectedException = new GameNotFoundException(invalidGameID);

            //--act
            var exception = Assert.ThrowsAsync<GameNotFoundException>(async () => await GamesClient.GetGameState(invalidGameID, TestEnvironmentSettings.BaseApiUrl));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public async Task It_Returns_The_Specified_Game()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName);

            //--act
            var gameState = await GamesClient.GetGameState(newGame.Id, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            gameState.Id.ShouldBe(newGame.Id);
            gameState.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
            gameState.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
        }

        //[Test]
        //public async Task It_Returns_The_Specified_Game_When_The_Game_Is_Well_Under_Way()
        //{
        //    //--arrange
        //    var newGame = await CreateValidGameForTesting(TestUserName, 1, 1);
        //    var firstPlayer = newGame.Players[0];

        //    SkillExpenditure skillExpenditure = new SkillExpenditure
        //    {
        //        IncreaseHypermutation = firstPlayer.MutationPoints
        //    };
        //    SkillExpenditureRequest skillExpenditureRequest =
        //        new SkillExpenditureRequest(newGame.Id, firstPlayer.Id, skillExpenditure);

        //    var result = await GamesClient.PushSkillExpenditures(skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl);


        //    //--act
        //    var gameState = await GamesClient.GetGameState(newGame.Id, TestEnvironmentSettings.BaseApiUrl);

        //    //--assert
        //    gameState.Id.ShouldBe(newGame.Id);
        //    gameState.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
        //    gameState.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
        //}
    }
}
