using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class GetPassiveSkillsTests : GamesApiClientTestBase
    {
        [Test]
        public async Task It_Returns_The_Passive_Skills()
        {
            //--arrange

            //--act
            var passiveSkills = await GamesClient.GetPassiveSkills(TestEnvironmentSettings.BaseApiUrl);

            //--assert
            passiveSkills.ShouldNotBeNull();

            passiveSkills.Count.ShouldBeGreaterThan(6);

            foreach (var passiveSkill in passiveSkills)
            {
                passiveSkill.Id.ShouldNotBe(0);
                passiveSkill.Name.Length.ShouldBeGreaterThan(0);
                passiveSkill.IncreasePerPoint.ShouldBeGreaterThan(0);
            }
        }

        [Test]
        public async Task It_Returns_The_Active_Skills()
        {
            //--arrange

            //--act
            var activeSkills = await GamesClient.GetActiveSkills(TestEnvironmentSettings.BaseApiUrl);

            //--assert
            activeSkills.ShouldNotBeNull();

            activeSkills.Count.ShouldBeGreaterThan(0);

            foreach (var activeSkill in activeSkills)
            {
                activeSkill.Id.ShouldNotBe(0);
                activeSkill.Name.Length.ShouldBeGreaterThan(0);
                activeSkill.NumberOfToastChanges.ShouldBeGreaterThan(0);
            }
        }
    }
}
