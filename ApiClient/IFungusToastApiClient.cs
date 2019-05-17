using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;

namespace ApiClient
{
    public interface IFungusToastApiClient
    {
        Task<GameModel> GetGameState(int gameId);
        Task<GameModel> CreateGame(NewGameRequest newGame);
        Task<SkillUpdateResult> PushSkillExpenditures(int gameId, string playerId, SkillExpenditureRequest skillExpenditureRequest);
        Task<List<Skill>> GetSkills();
    }
}