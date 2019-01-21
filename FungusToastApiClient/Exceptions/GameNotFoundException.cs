using System;

namespace FungusToastApiClient.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(int gameId) : base($"No game state information could be found for game with id '{gameId}'")
        {
        }
    }
}