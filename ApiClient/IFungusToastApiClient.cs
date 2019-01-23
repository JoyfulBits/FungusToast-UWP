using System.Threading.Tasks;
using ApiClient.Models;

namespace ApiClient
{
    public interface IFungusToastApiClient
    {
        Task<GameState> GetGameState(int gameId);
        Task<GameState> CreateGame(NewGameRequest newGame);
    }
}