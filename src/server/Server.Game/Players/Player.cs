using System;
using Server.Game.Levels;

namespace Server.Game.Players
{
    public class Player
    {
        public string Id { get; }
        public string Name { get; }
        public int GameProgress { get; private set; }
        public Game ActiveGame { get; private set; }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public Game StartNextLevel(Func<int, LevelData> levelDataResolver, Action<Game> processUpdate)
        {
            if (ActiveGame != null && !ActiveGame.IsFinished)
                throw new Exception("There's still an active game");

            var nextLevel = GameProgress + 1;
            var nextLevelData = levelDataResolver(nextLevel);
            ActiveGame = new Game();
            ActiveGame.LoadLevelData(nextLevelData);
            ActiveGame.Join(this);
            ActiveGame.StartTimer(processUpdate);
            return ActiveGame;
        }

        public void FinishedGame()
        {
            GameProgress += 1;
            ActiveGame = null;
        }
    }
}