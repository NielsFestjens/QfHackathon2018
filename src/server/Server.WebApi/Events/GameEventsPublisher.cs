using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Server.Game.Players;
using Server.WebApi.Hubs;
using Server.WebApi.SignalR;

namespace Server.WebApi
{
    public class GameEventsPublisher : IGameEvents
    {
        private readonly IHubContext<ClientsHub> _clientsHub;
        private readonly IHubContext<AdminsHub> _adminsHub;

        public GameEventsPublisher(IHubContext<ClientsHub> clientsHub, IHubContext<AdminsHub> adminsHub)
        {
            _clientsHub = clientsHub;
            _adminsHub = adminsHub;
        }

        private IClientProxy Admins => _adminsHub.Clients.All;
        private IClientProxy Spectators => _clientsHub.Clients.Group("Spectators");

        private IClientProxy AdminsAndSpectators => Admins.And(Spectators);
        private IClientProxy Client(string id) => _clientsHub.Clients.Client(id);
        private IClientProxy AdminsAndSpectatorsAndClient(string id) => AdminsAndSpectators.And(Client(id));

        public Task OnPlayerConnected(Player player)
            => AdminsAndSpectatorsAndClient(player.Id).SendAsync("PlayerConnected", new { connectionId = player.Id, name = player.Name });

        public Task OnPlayerDisconnected(string connectionId)
            => AdminsAndSpectators.SendAsync("PlayerDisconnected", new { connectionId });

        public Task OnSpectatorConnected(string connectionId)
            => Admins.SendAsync("SpectatorConnected", new { connectionId });

        public Task OnSpectatorDisconnected(string connectionId)
            => Admins.SendAsync("SpectatorDisconnected", new { connectionId });

        public Task OnGameStarted(Game.Players.Game game)
            => AdminsAndSpectatorsAndClient(game.PlayerId).SendAsync("GameStarted", new {game.PlayerId, game.Level});
    }
}