using ApiClient.Models;

namespace ApiClient.Exceptions
{
    public class SkillUpdateResult
    {
        public bool NextRoundAvailable { get; set; }
        public PlayerState UpdatedPlayer { get; set; }
    }
}