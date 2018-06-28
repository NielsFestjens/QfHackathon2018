using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Server.WebApi.Hubs
{
    public class AdminsHub : Hub
    {
        private readonly IHubContext<ClientsHub> _clientsHub;

        public AdminsHub(IHubContext<ClientsHub> clientsHub)
        {
            _clientsHub = clientsHub;
        }

        public Task Kick(string connectionId)
        {
            return _clientsHub.Clients.Client(connectionId).SendAsync("Kicked");
        }
    }
}