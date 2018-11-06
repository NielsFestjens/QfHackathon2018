using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Server.Game.Levels;
using Server.Game.Players;

namespace Server.Game
{
    public class Game
    {
        private readonly Random _random;
        private readonly Dictionary<GamePlayer, Move> _moves = new Dictionary<GamePlayer, Move>();
        private Timer _timer;

        public List<GamePlayer> Players { get; } = new List<GamePlayer>();
        public List<TileData> Tiles { get; } = new List<TileData>();

        public int Level { get; }
        public string Name { get; }
        public int Rows { get; }
        public int Columns { get; }
        public List<SpawnArea> SpawnAreas { get; set; }

        public bool IsFinished { get; private set; }

        public Game(LevelData levelData, Player player, Action<Game> processUpdate)
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

            StartTimer(processUpdate);
        }

        private void StartTimer(Action<Game> processUpdate)
        {
            _timer = new Timer(state => processUpdate(this), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
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
            Tiles.Add(new TileData(player.Column, player.Row, new TileContent(TileContentType.Character, player.Player.Id)));
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
            if (content.Type == TileContentType.Character && content.Id != player.Player.Id)
                return new TileContent(TileContentType.Enemy);

            return content;
        }

        public void SetNextMove(Player player, Move move)
        {
            var gamePlayer = Players.Single(x => x.Player == player);
            _moves[gamePlayer] = move;
        }

        public void ProcessUpdate()
        {
            foreach (var move in _moves)
            {
                ProcessMove(move.Key, move.Value);
            }
            _moves.Clear();
        }

        private void ProcessMove(GamePlayer player, Move move)
        {
            var (desiredColumn, desiredRow) = GetDesiredPosition(player, move);
            if (!CanWalkOn(desiredColumn, desiredRow, player))
                return;

            player.Column = desiredColumn;
            player.Row = desiredRow;
        }

        private bool CanWalkOn(int column, int row, GamePlayer player)
        {
            return !Tiles.Contents(column, row).Any(x => x.Type == TileContentType.Obstacle);
        }

        private (int, int) GetDesiredPosition(GamePlayer player, Move move)
        {
            var (offSetColumn, offsetRow) = move.MapToOffset();
            var desiredColumn = (player.Column + offSetColumn).Between(0, Columns);
            var desiredRow = (player.Row + offsetRow).Between(0, Rows);
            return (desiredColumn, desiredRow);
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
        public TileContentType Type { get; }
        public string Id { get; }

        public TileContent(TileContentType type, string id = null)
        {
            Type = type;
            Id = id;
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

    public enum Move
    {
        Up,
        Right,
        Down,
        Left
    }

    public static class MoveExtensions
    {
        public static (int, int) MapToOffset(this Move move)
        {
            switch (move)
            {
                case Move.Up: return (0, 1);
                case Move.Right: return (1, 0);
                case Move.Down: return (0, -1);
                case Move.Left: return (-1, 0);
                default: return (0, 0);
            }
        }
    }

    public static class IntExtensions
    {
        public static int Between(this int input, int min, int max)
        {
            return Math.Max(Math.Min(input, max), min);
        }
    }

    public static class TileDataExtensions
    {
        public static IEnumerable<TileData> For(this IEnumerable<TileData> tiles, int column, int row)
        {
            return tiles.Where(x => x.Column == column && x.Row == row);
        }

        public static IEnumerable<TileContent> Contents(this IEnumerable<TileData> tiles, int column, int row)
        {
            return tiles.For(column, row).SelectMany(x => x.Content);
        }
    }
}