using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace ApiClient.Tests.GamesApiClientTests
{
    [TestFixture]
    public class GetSkillsTests : GamesApiClientTestBase
    {
        [Test]
        public async Task It_Returns_The_SKills()
        {
            //--arrange

            //--act
            var skills = await GamesClient.GetSkills(TestEnvironmentSettings.BaseApiUrl);

            //--assert
            skills.ShouldNotBeNull();
            skills.Count.ShouldBeGreaterThan(4);

            foreach (var skill in skills)
            {
                skill.Id.ShouldNotBe(0);
                skill.Name.Length.ShouldBeGreaterThan(0);
                skill.IncreasePerPoint.ShouldBeGreaterThan(0);
            }
        }
    }
}
