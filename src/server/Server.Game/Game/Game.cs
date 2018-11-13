using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game.Levels;
using Server.Game.Players;

namespace Server.Game
{
    public class Game : IDisposable
    {
        private GameMoveLogic _gameMoveLogic;
        private readonly Random _random;
        private ActionTimer _timer;

        public List<GamePlayer> Players { get; } = new List<GamePlayer>();
        public List<TileContent> Tiles { get; } = new List<TileContent>();

        public int Level { get; private set; }
        public string Name { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public List<SpawnArea> SpawnAreas { get; private set; } = new List<SpawnArea>();

        public bool IsFinished { get; set; }

        public Game()
        {
            _gameMoveLogic = new GameMoveLogic();
            _random = new Random();
        }

        public void LoadLevelData(LevelData levelData)
        {
            if (levelData == null)
                return;

            Level = levelData.Level;
            Name = levelData.Name;
            Columns = levelData.Grid.Cols;
            Rows = levelData.Grid.Rows;
            SpawnAreas = levelData.Grid.SpwawnAreas.Select(x => new SpawnArea(x)).ToList();

            foreach (var coords in levelData.Grid.Walls)
            {
                Tiles.Add(new ObstacleTileContent(coords[0], coords[1], "Wall"));
            }
            
            Tiles.Add(new FinishTileContent(levelData.Grid.Finish[0], levelData.Grid.Finish[1], this));
        }

        public void Join(Player player)
        {
            var gamePlayer = new GamePlayer(player, this);
            Players.Add(gamePlayer);
            Spawn(gamePlayer);
        }

        private void Spawn(GamePlayer player)
        {
            if (!SpawnAreas.Any())
                return;

            (player.Column, player.Row) = SpawnAreas[_random.Next(SpawnAreas.Count)].GetRandomPoint(_random);
            var playerTile = new PlayerTileContent(player.Column, player.Row, player);
            player.Tile = playerTile;
            Tiles.Add(playerTile);
        }

        public void StartTimer(Action<Game> processUpdate)
        {
            _timer = new ActionTimer(() => processUpdate(this), TimeSpan.FromMilliseconds(100));
            _timer.Start();
        }

        public List<TileData> GetViewportFor(GamePlayer player)
        {
            var minCol = player.Column - player.ViewDistance;
            var maxCol = player.Column + player.ViewDistance;
            var minRow = player.Row - player.ViewDistance;
            var maxRow = player.Row + player.ViewDistance;

            return Tiles
                .Where(tile => tile.Column >= minCol && tile.Column <= maxCol && tile.Row >= minRow && tile.Row <= maxRow)
                .Select(x => new TileData(x.Column, x.Row, x.MapToData(player)))
                .ToList();
        }

        public void SetNextMove(Player player, Move move)
        {
            var gamePlayer = Players.SingleOrDefault(x => x.Player == player);
            if (gamePlayer == null)
                return;

            _gameMoveLogic.SetNextMove(gamePlayer, move);
        }

        public void ProcessUpdate()
        {
            _gameMoveLogic.ProcessMoves();
            ProcessEffects();
        }

        private void ProcessEffects()
        {
            foreach (var tile in Tiles)
            {
                tile.Process();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;

            _gameMoveLogic.Dispose();
            _gameMoveLogic = null;

            foreach (var player in Players)
            {
                player.Dispose();
            }
            Players.Clear();

            Tiles.Clear();
        }
    }
}