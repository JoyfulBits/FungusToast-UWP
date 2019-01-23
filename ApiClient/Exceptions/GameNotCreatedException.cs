using System;
using ApiClient.Models;
using Newtonsoft.Json;

namespace ApiClient.Exceptions
{
    public class GameNotCreatedException : Exception
    {
        public GameNotCreatedException(NewGameRequest newGame) : base($"Failed to create game with the following data: '{JsonConvert.SerializeObject(newGame)}'")
        {
        }
    }
}