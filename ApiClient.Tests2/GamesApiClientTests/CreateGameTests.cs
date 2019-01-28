﻿using System.Threading.Tasks;
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
            var newGame = new NewGameRequest(2, 1);

            //--act
            var result = await GamesClient.CreateGame(newGame, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.NumberOfHumanPlayers.ShouldBe(newGame.NumberOfHumanPlayers);
            result.NumberOfAiPlayers.ShouldBe(newGame.NumberOfAiPlayers);
            result.NumberOfRows.ShouldBe(50);
            result.NumberOfColumns.ShouldBe(50);
            result.Id.ShouldBeGreaterThan(0);
            result.Status.ShouldBe("Not Started");
        }

        [Test]
        public async Task It_Automatically_Starts_The_Game_If_There_Is_Only_One_Human_Player()
        {
            //--arrange
            var newGame = new NewGameRequest(1, 1);

            //--act
            var result = await GamesClient.CreateGame(newGame, TestEnvironmentSettings.BaseApiUrl);

            //--assert
            result.Status.ShouldBe("In Progress");
        }
    }
}
