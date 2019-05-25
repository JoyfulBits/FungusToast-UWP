using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class PushSkillExpendituresTests : GamesApiClientTestBase
    {
        [Test]
        public async Task It_Returns_A_400_Bad_Request_If_The_Player_Attempted_To_Push_Too_Many_Active_Cell_Changes()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName, 2, 0);
            var firstPlayer = newGame.Players[0];

            var skillExpenditureRequest = new SkillExpenditureRequest(firstPlayer.Id);
            skillExpenditureRequest.AddMoistureDroplet(1);
            skillExpenditureRequest.AddMoistureDroplet(2);
            skillExpenditureRequest.AddMoistureDroplet(3);
            skillExpenditureRequest.AddMoistureDroplet(4);

            //--act
            var exception = Assert.ThrowsAsync<ApiException>(async () => await GamesClient.PushSkillExpenditures(newGame.Id, firstPlayer.Id, skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl));

            //--assert
            exception.ResponseStatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task It_Returns_The_Updated_Player()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName, 2, 0);
            var firstPlayer = newGame.Players[0];

            var skillExpenditureRequest = new SkillExpenditureRequest(firstPlayer.Id);
            skillExpenditureRequest.IncreaseBudding();

            //--act
            var result = await GamesClient.PushSkillExpenditures(newGame.Id, firstPlayer.Id, skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.UpdatedPlayer.MutationPoints.ShouldBe(firstPlayer.MutationPoints - 1);
        }

        [Test]
        public async Task It_Returns_Next_Round_Not_Available_If_Not_All_Players_Have_Pushed_Their_Points()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName, 2, 0);
            var firstPlayer = newGame.Players[0];

            var skillExpenditureRequest = new SkillExpenditureRequest(firstPlayer.Id);
            for (int i = 0; i < firstPlayer.MutationPoints; i++)
            {
                skillExpenditureRequest.IncreaseRegeneration();
            }

            //--act
            var result = await GamesClient.PushSkillExpenditures(newGame.Id, firstPlayer.Id, skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NextRoundAvailable.ShouldBe(false);
        }

        [Test]
        public async Task It_Returns_Next_Round_Available_If_All_Players_Have_Pushed_Their_Points()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName, 1, 1);
            var firstPlayer = newGame.Players.First(x => x.Human);

            var skillExpenditureRequest = new SkillExpenditureRequest(firstPlayer.Id);
            for (int i = 0; i < firstPlayer.MutationPoints; i++)
            {
                skillExpenditureRequest.IncreaseRegeneration();
            }

            //--act
            var result = await GamesClient.PushSkillExpenditures(newGame.Id, firstPlayer.Id, skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NextRoundAvailable.ShouldBe(true);
        }
    }
}
