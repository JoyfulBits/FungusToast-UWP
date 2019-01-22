using System;
using FungusToastApiClientLibrary.Models;
using Newtonsoft.Json;

namespace FungusToastApiClientLibrary.Exceptions
{
    public class GameNotCreatedException : Exception
    {
        public GameNotCreatedException(NewGameRequest newGame) : base($"Failed to create game with the following data: '{JsonConvert.SerializeObject(newGame)}'")
        {
        }
    }
}