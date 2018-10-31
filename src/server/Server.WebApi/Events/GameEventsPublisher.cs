

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game;
using Server.WebApi.Hubs;

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

        public Task OnPlayerConnected(string connectionId, string name)
        {
            var adminsSendTask = Admins.SendAsync("PlayerConnected", new { connectionId, name });
            var spectatorsSendTask = Spectators.SendAsync("PlayerConnected", new { connectionId, name });

            return Task.WhenAll(adminsSendTask, spectatorsSendTask);
        }

        public Task OnPlayerDisconnected(string connectionId)
        {
            var adminsSendTask = Admins.SendAsync("PlayerConnected", new { connectionId });
            var spectatorsSendTask = Spectators.SendAsync("PlayerConnected", new { connectionId });

            return Task.WhenAll(adminsSendTask, spectatorsSendTask);
        }

        public Task OnSpectatorConnected(string connectionId)
            => Admins.SendAsync("SpectatorConnected", new { connectionId });

        public Task OnSpectatorDisconnected(string connectionId)
            => Admins.SendAsync("SpectatorDisconnected", new { connectionId });
    }
}