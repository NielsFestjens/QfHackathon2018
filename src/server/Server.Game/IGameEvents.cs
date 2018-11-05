using System.Threading.Tasks;
using Server.Game.Players;

namespace Server.Game
{
    public interface IGameEvents
    {
        Task OnPlayerConnected(Player player);
        Task OnPlayerDisconnected(string connectionId);
        Task OnSpectatorConnected(string connectionId);
        Task OnSpectatorDisconnected(string connectionId);
        Task OnGameStarted(Players.Game game);
    }
}