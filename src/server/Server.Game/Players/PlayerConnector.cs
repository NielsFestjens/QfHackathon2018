using System.Linq;
using Server.Game.Levels;

namespace Server.Game.Players
{
    public interface IPlayerConnector
    {
        bool IsConnected(string id);
        void Connect(string id, string name);
        void Disconnect(string id);
        void StartNextLevel(Player player);
        void MakeMove(string id, Move move);
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
            var game = player.StartNextLevel(_levelManager.LoadLevelData, ProcessUpdate);
            _events.OnGameStarted(game);

            ProcessUpdate(game);
        }

        public void MakeMove(string id, Move move)
        {
            var player = _playerManager.GetPlayer(id);
            var game = player.ActiveGame;
            game?.SetNextMove(player, move);
        }

        public void ProcessUpdate(Game game)
        {
            game.ProcessUpdate();
            _events.OnGameUpdated(game);

            if (game.IsFinished)
            {
                var players = game.Players.Select(x => x.Player).ToList();
                game.Dispose();
                foreach (var player in players)
                {
                    FinishedGame(player, game);
                }
            }
        }

        public void FinishedGame(Player player, Game game)
        {
            player.FinishedGame();
            _events.OnGameFinished(game);
            StartNextLevel(player);
        }

        public void Disconnect(string id)
        {
            _events.OnPlayerDisconnected(id);
        }
    }
}