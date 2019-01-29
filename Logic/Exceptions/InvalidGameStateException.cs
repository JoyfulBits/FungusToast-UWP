using System;

namespace Logic.Exceptions
{
    public class InvalidGameStateException : Exception
    {
        public InvalidGameStateException(string message) : base(message)
        {
        }
    }
}