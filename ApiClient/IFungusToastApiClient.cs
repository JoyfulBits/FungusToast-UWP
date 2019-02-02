using System.Threading.Tasks;
using ApiClient.Exceptions;
using ApiClient.Models;

namespace ApiClient
{
    public interface IFungusToastApiClient
    {
        Task<GameModel> GetGameState(int gameId, MockOption? mockOption = null);
        Task<GameModel> CreateGame(NewGameRequest newGame, bool returnMock = false);
        Task<SkillUpdateResult> PushSkillExpenditures(SkillExpenditureRequest skillExpenditureRequest, bool? mockNextRoundAvailable = null);
    }
}