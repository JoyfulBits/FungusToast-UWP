using System.Collections.Generic;

namespace ApiClient.Models
{
    public class GameState
    {
        public Dictionary<int, FungalCell> Cells { get; set; } = new Dictionary<int, FungalCell>();
    }
}