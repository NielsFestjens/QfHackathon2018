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

        public async Task Connect(string name, Guid? apiKey)
        {
            var connectionId = Context.ConnectionId;
            // var isSpectator = apiKey != null && ValidApiKeys.Contains(apiKey.ToString());

            await _adminsHub.Clients.All.SendAsync("ClientConnected", connectionId, name);

            // 1. Add client to list of connected 
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _adminsHub.Clients.All.SendAsync("ClientDisconnected", Context.ConnectionId);
        }
    }
}