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
            var fileName = $"App_Data\\levels\\{level}.json";
            if (!File.Exists(fileName))
                return null;

            var levelDataJson = File.ReadAllText(fileName);
            var levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);
            levelData.Level = level;
            return levelData;
        }
    }
}