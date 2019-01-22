using System.Threading.Tasks;
using FungusToastApiClient.Models;

namespace FungusToastApiClient
{
    public interface IFungusToastApiClient
    {
        Task<GameState> GetGameState(int gameId);
        Task<GameState> CreateGame(NewGameRequest newGame);
    }
}