using System;

namespace Server.Game.Players
{
    public class Player
    {
        private Game _activeGame;

        public string Id { get; }
        public string Name { get; }
        public int GameProgress { get; private set; }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public Game StartNextLevel(Func<int, LevelData> levelDataResolver)
        {
            if (_activeGame != null && !_activeGame.IsFinished)
                throw new Exception("There's still an active game");

            var nextLevel = GameProgress + 1;
            var nextLevelData = levelDataResolver(nextLevel);
            _activeGame = new Game(Id, nextLevelData);
            return _activeGame;
        }
    }
}