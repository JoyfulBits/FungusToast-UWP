using System;
using System.Collections.Generic;
using System.Text;

namespace FungusToastApiClient.Models
{
    public class GameState
    {
        public int Id { get; set; }
        public int NumberOfHumanPlayers { get; set; }
        public int NumberOfAiPlayers { get; set; }
    }
}
