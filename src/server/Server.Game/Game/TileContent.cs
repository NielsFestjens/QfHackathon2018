using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Server.Game
{
    [DebuggerDisplay("[{Column}, {Row}]: {Type} - {Name}")]
    public abstract class TileContent
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public abstract TileContentType Type { get; }
        public string Name { get; }
        public virtual bool IsObstacle => false;

        protected TileContent(int column, int row, string name)
        {
            Column = column;
            Row = row;
            Name = name;
        }

        public virtual TileContentData MapToData(GamePlayer player)
        {
            return new TileContentData(Type, Name);
        }

        public virtual void Process()
        {
        }
    }

    public static class TileContentExtensions
    {
        public static IEnumerable<TileContent> For(this IEnumerable<TileContent> tiles, int column, int row)
        {
            return tiles.Where(x => x.Column == column && x.Row == row);
        }
    }

    public class ObstacleTileContent : TileContent
    {
        public override TileContentType Type => TileContentType.Obstacle;
        public override bool IsObstacle => true;

        public ObstacleTileContent(int column, int row, string name) : base(column, row, name)
        {
        }
    }

    public class PlayerTileContent : TileContent
    {
        public override TileContentType Type => TileContentType.Character;

        public GamePlayer Player { get; }

        public PlayerTileContent(int column, int row, GamePlayer player) : base(column, row, player.Player.Name)
        {
            Player = player;
        }

        public override TileContentData MapToData(GamePlayer player)
        {
            var data = base.MapToData(player);
            data.Type = Player == player ? TileContentType.Friendly : TileContentType.Enemy;
            data.Id = Player.Player.Id;
            return data;
        }
    }

    public class FinishTileContent : TileContent
    {
        private readonly Game _game;

        public FinishTileContent(int column, int row,Game game) : base(column, row, "Finish")
        {
            _game = game;
        }

        public override TileContentType Type => TileContentType.Finish;

        public override void Process()
        {
            if (_game.Players.Any(x => x.Column == Column && x.Row == Row))
            {
                _game.IsFinished = true;
            }
        }
    }
}