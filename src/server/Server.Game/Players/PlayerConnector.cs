using System;
using Server.Game.Levels;

namespace Server.Game.Players
{
    public interface IPlayerConnector
    {
        bool IsConnected(string id);
        void Connect(string id, string name);
        void Disconnect(string id);
        void StartNextLevel(Player player);
    }

    public class PlayerConnector : IPlayerConnector
    {
        private readonly IGameEvents _events;
        private readonly IPlayerManager _playerManager;
        private readonly ILevelManager _levelManager;

        public PlayerConnector(IGameEvents events, IPlayerManager playerManager, ILevelManager levelManager)
        {
            _events = events;
            _playerManager = playerManager;
            _levelManager = levelManager;
        }

        public bool IsConnected(string id)
        {
            return true;
        }

        public void Connect(string id, string name)
        {
            var player = _playerManager.CreatePlayer(id, name);
            _events.OnPlayerConnected(player);

            StartNextLevel(player);
        }

        public void StartNextLevel(Player player)
        {
            var game = player.StartNextLevel(_levelManager.LoadLevelData);
            _events.OnGameStarted(game);

            ProcessUpdate(game);
        }

        public void ProcessUpdate(Game game)
        {
            _events.OnGameUpdated(game);
        }

        public void Disconnect(string id)
        {
            _events.OnPlayerDisconnected(id);
        }
    }
}