using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Server.WebApi.Hubs
{
    public class ClientsHub : Hub
    {
        private readonly IHubContext<AdminsHub> _adminsHub;

        public ClientsHub(IHubContext<AdminsHub> adminsHub)
        {
            _adminsHub = adminsHub;
        }

        public Task Connect(string name)
        {
            var connectionId = Context.ConnectionId;
            if (name == "KickMe")
                return Clients.Client(connectionId).SendAsync("Kicked");

            return _adminsHub.Clients.All.SendAsync("ClientConnected", connectionId, name);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _adminsHub.Clients.All.SendAsync("ClientDisconnected", Context.ConnectionId);
        }
    }
}