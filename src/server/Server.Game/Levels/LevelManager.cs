namespace Server.Game.Players
{
    public interface ILevelManager
    {
        LevelData LoadLevelData(int level);
    }

    public class LevelManager : ILevelManager
    {
        public LevelData LoadLevelData(int level)
        {
            return new LevelData
            {
                Level = level
            };
        }
    }
}