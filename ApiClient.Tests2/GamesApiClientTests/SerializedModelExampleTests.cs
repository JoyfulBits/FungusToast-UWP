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
        public void Example_Model_Json_For_New_Game_Not_Started()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForNotStartedGame();

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_Model_Json_For_New_That_Just_Started()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForJustStartedGame();

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_Model_Json_For_Game_That_Is_In_Progress()
        {
            //--arrange
            var gameModel = MockDataBuilder.MakeMockGameModelForGameThatIsWellUnderWay();

            //--act
            var jsonObject = JsonConvert.SerializeObject(gameModel, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            Debug.WriteLine(jsonObject);
        }

        [Test]
        public void Example_SkillExpenditure()
        {
            //--arrange
            var request = new SkillExpenditureRequest(1, "player id 1", new SkillExpenditure
            {
                AntiApoptosisPoints = 1,
                BuddingPoints = 2,
                HypermutationPoints = 0,
                MycotoxicityPoints = 1,
                RegenerationPoints = 3
            });

            //--act
            var jsonObject = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            //--assert
            //--assert
            Debug.WriteLine(jsonObject);
        }
    }
}
