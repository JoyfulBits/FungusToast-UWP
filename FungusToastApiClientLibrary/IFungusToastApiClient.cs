using System.Threading.Tasks;
using FungusToastApiClientLibrary.Models;

namespace FungusToastApiClientLibrary
{
    public interface IFungusToastApiClient
    {
        Task<GameState> GetGameState(int gameId);
        Task<GameState> CreateGame(NewGameRequest newGame);
    }
}