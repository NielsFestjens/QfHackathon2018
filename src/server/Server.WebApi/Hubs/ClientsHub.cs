using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game.Players;
using Server.Game.Spectators;

namespace Server.WebApi.Hubs
{
    public class ClientsHub : Hub
    {
        private readonly IPlayerConnector _playerConnector;
        private readonly ISpectatorConnector _spectatorConnector;

        public ClientsHub(
            IPlayerConnector playerConnector,
            ISpectatorConnector spectatorConnector)
        {
            _playerConnector = playerConnector;
            _spectatorConnector = spectatorConnector;
        }

        public Task Connect(string name, Guid? apiKey)
        {
            if (apiKey.HasValue)
            {
                if (!_spectatorConnector.IsAuthorized(apiKey.Value))
                {
                    Context.Abort();
                }
                else
                {
                    _spectatorConnector.Connect(Context.ConnectionId);
                }
            }
            else
            {
                _playerConnector.Connect(Context.ConnectionId, name);
            }
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_spectatorConnector.IsConnected(Context.ConnectionId))
            {
                _spectatorConnector.Disconnect(Context.ConnectionId);
            }
            if (_playerConnector.IsConnected(Context.ConnectionId))
            {
                _playerConnector.Disconnect(Context.ConnectionId);
            }
            return Task.CompletedTask;
        }
    }
}