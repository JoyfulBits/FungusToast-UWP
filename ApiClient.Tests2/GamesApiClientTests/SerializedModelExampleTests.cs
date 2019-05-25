using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ApiClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace ApiClient.Tests.GamesApiClientTests
{
    [Category("Integration")]
    [TestFixture]
    public class SerializedModelExampleTests
    {
        [Test]
        public void Example_Json_For_Creating_A_Game()
        {
            //--arrange
            var newGameRequest = new NewGameRequest("jake", 1, 2);

            //--act
            var jsonString = JsonConvert.SerializeObject(newGameRequest, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonString);
        }

        [Test]
        public void Example_Model_Json_For_New_Game_Not_Started()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForNotStartedGame();

            //--act
            var jsonString = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonString);
        }

        [Test]
        public void Example_Model_Json_For_New_That_Just_Started()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForJustStartedGame();

            //--act
            var jsonString = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonString);
        }

        [Test]
        public void Example_Model_Json_For_Game_That_Is_In_Progress()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForGameThatIsWellUnderWay();

            //--act
            var jsonString = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonString);
        }

        [Test]
        public void Example_SkillExpenditure()
        {
            //--arrange
            var request = new SkillExpenditureRequest("31");
            request.IncreaseRegeneration();
            request.IncreaseBudding();
            request.IncreaseBudding();
            request.IncreaseAntiApoptosis();
            request.IncreaseHypermutation();
            request.IncreaseMycotoxicity();
            request.IncreaseHydrophilia();
            request.AddMoistureDroplet(1);
            request.AddMoistureDroplet(351);
            request.AddMoistureDroplet(591);

            //--act
            var jsonString = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            //--assert
            Debug.WriteLine(jsonString);
        }
    }
}
