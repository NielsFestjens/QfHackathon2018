using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game.Levels;
using Server.Game.Players;

namespace Server.Game
{
    public class Game
    {
        private Random _random;
        public List<GamePlayer> Players { get; } = new List<GamePlayer>();
        public List<TileData> Tiles { get; } = new List<TileData>();

        public int Level { get; }
        public string Name { get; }
        public int Rows { get; }
        public int Columns { get; }
        public List<SpawnArea> SpawnAreas { get; set; }

        public bool IsFinished { get; private set; }

        public Game(LevelData levelData, Player player)
        {
            _random = new Random();
            Level = levelData.Level;
            Name = levelData.Name;
            Rows = levelData.Grid.Rows;
            Columns = levelData.Grid.Cols;
            SpawnAreas = levelData.Grid.SpwawnAreas.Select(x => new SpawnArea(x)).ToList();
            foreach (var coords in levelData.Grid.Walls)
            {
                Tiles.Add(new TileData(coords[0], coords[1], new TileContent(TileContentType.Obstacle)));
            }

            Join(player);
        }


        public void Join(Player player)
        {
            var gamePlayer = new GamePlayer(player);
            Players.Add(gamePlayer);
            Spawn(gamePlayer);
        }

        private void Spawn(GamePlayer player)
        {
            (player.Column, player.Row) = SpawnAreas[_random.Next(SpawnAreas.Count)].GetRandomPoint(_random);
            Tiles.Add(new TileData(player.Column, player.Row, new TileContent(TileContentType.Character)));
        }

        public List<TileData> GetViewportFor(GamePlayer player)
        {
            var minRow = player.Row - player.ViewDistance;
            var maxRow = player.Row + player.ViewDistance;
            var minCol = player.Column - player.ViewDistance;
            var maxCol = player.Column + player.ViewDistance;

            return Tiles
                .Where(tile => tile.Row >= minRow && tile.Row <= maxRow && tile.Column >= minCol && tile.Column <= maxCol)
                .Select(x => new TileData(x.Row, x.Column, MapTile(x.Content, player).ToArray()))
                .ToList();
        }

        private IEnumerable<TileContent> MapTile(IEnumerable<TileContent> tileData, GamePlayer player)
        {
            return tileData.Select(x => MapContent(x, player));
        }

        private TileContent MapContent(TileContent content, GamePlayer player)
        {
            if (content.Type == TileContentType.Character)
                return new TileContent(TileContentType.Enemy);

            return content;
        }
    }

    public class SpawnArea
    {
        public int MinRow { get; set; }
        public int MaxRow { get; set; }
        public int MinCol { get; set; }
        public int MaxCol { get; set; }

        public SpawnArea(int[] location)
        {
            MinRow = location[0];
            MinCol = location[1];
            MaxRow = location[2];
            MaxCol = location[3];
        }

        public (int, int) GetRandomPoint(Random random)
        {
            return (random.Next(MinRow, MaxRow), random.Next(MinCol, MaxCol));
        }
    }

    public class GamePlayer
    {
        public Player Player { get; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int ViewDistance { get; } = 3;

        public GamePlayer(Player player)
        {
            Player = player;
        }
    }

    public class TileData
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public List<TileContent> Content { get; set; }

        public TileData(int row, int column, params TileContent[] content)
        {
            Row = row;
            Column = column;
            Content = content.ToList();
        }
    }

    public class TileContent
    {
        public TileContentType Type { get; set; }

        public TileContent(TileContentType type)
        {
            Type = type;
        }
    }

    public enum TileContentType
    {
        Unknown = 0,
        Obstacle = 1,
        Object = 2,
        Character = 3,
        Enemy = 4,
        Friendly = 5
    }
}