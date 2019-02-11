using System.Linq;
using System.Threading.Tasks;
using ApiClient.Models;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class PushSkillExpendituresTests : GamesApiClientTestBase
    {
        [Test]
        public async Task It_Returns_Next_Round_Not_Available_If_Not_All_Players_Have_Pushed_Their_Points()
        {
            //--arrange
            var newGame = await CreateValidGameForTesting(TestUserName, 2, 0);
            var firstPlayer = newGame.Players[0];

            var skillExpenditureRequest = new SkillExpenditureRequest
            {
                RegenerationPoints = firstPlayer.MutationPoints - 1,
                BuddingPoints = 1
            };

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
            var firstPlayer = newGame.Players.FirstOrDefault(x => x.Human);

            var skillExpenditureRequest = new SkillExpenditureRequest
            {
                RegenerationPoints = firstPlayer.MutationPoints
            };

            //--act
            var result = await GamesClient.PushSkillExpenditures(newGame.Id, firstPlayer.Id, skillExpenditureRequest, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NextRoundAvailable.ShouldBe(true);
        }
    }
}
