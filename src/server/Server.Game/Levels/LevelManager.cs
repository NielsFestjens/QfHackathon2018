using System.IO;
using Newtonsoft.Json;

namespace Server.Game.Levels
{
    public interface ILevelManager
    {
        LevelData LoadLevelData(int level);
    }

    public class LevelManager : ILevelManager
    {
        public LevelData LoadLevelData(int level)
        {
            var levelDataJson = File.ReadAllText($"App_Data\\levels\\{level}.json");
            var levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);
            levelData.Level = level;
            return levelData;
        }
    }
}