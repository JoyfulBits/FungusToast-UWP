using System;
using System.Collections.Generic;
using System.Text;
using FungusToastApiClient.Models;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Shouldly;

namespace FungusToastApiClient.Tests.GamesApiClientTests
{
    [TestFixture()]
    public class CreateGameTests : GamesApiClientTestBase
    {
        [Test]
        public async void It_Creates_The_Game()
        {
            //--arrange
            var newGame = new NewGameRequest(2, 1);

            //--act
            var result = await GamesClient.CreateGame(newGame, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
            result.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
        }
    }
}
