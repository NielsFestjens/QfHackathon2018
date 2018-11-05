namespace Server.Game.Players
{
    public class Game
    {
        public string PlayerId { get; }
        public int Level { get; set; }
        public bool IsFinished { get; set; }

        public Game(string playerId, LevelData levelData)
        {
            PlayerId = playerId;
            Level = levelData.Level;
        }
    }
}