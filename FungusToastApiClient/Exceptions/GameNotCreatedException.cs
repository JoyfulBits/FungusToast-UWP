using System;
using FungusToastApiClient.Models;
using Newtonsoft.Json;

namespace FungusToastApiClient.Exceptions
{
    public class GameNotCreatedException : Exception
    {
        public GameNotCreatedException(NewGameRequest newGame) : base($"Failed to create game with the following data: '{JsonConvert.SerializeObject(newGame)}'")
        {
        }
    }
}