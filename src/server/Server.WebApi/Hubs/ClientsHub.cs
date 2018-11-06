using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Game;
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

        public Task Connect(ConnectCommand command)
        {
            if (command.ApiKey.HasValue)
            {
                if (!_spectatorConnector.IsAuthorized(command.ApiKey.Value))
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
                _playerConnector.Connect(Context.ConnectionId, command.Name);
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

        public Task MoveUp()
        {
            _playerConnector.MakeMove(Context.ConnectionId, Move.Up);
            return Task.CompletedTask;
        }

        public Task MoveRight()
        {
            _playerConnector.MakeMove(Context.ConnectionId, Move.Right);
            return Task.CompletedTask;
        }

        public Task MoveDown()
        {
            _playerConnector.MakeMove(Context.ConnectionId, Move.Down);
            return Task.CompletedTask;
        }

        public Task MoveLeft()
        {
            _playerConnector.MakeMove(Context.ConnectionId, Move.Left);
            return Task.CompletedTask;
        }
    }

    public class ConnectCommand
    {
        public string Name { get; set; }
        public Guid? ApiKey { get; set; }
    }
}