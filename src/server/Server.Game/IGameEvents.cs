using System.Threading.Tasks;

namespace Server.Game
{
    public interface IGameEvents
    {
        Task OnPlayerConnected(string connectionId, string name);
        Task OnPlayerDisconnected(string connectionId);
        Task OnSpectatorConnected(string connectionId);
        Task OnSpectatorDisconnected(string connectionId);
    }
}