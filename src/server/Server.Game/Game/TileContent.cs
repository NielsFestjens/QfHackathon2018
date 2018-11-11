using System.Collections.Generic;
using System.Linq;

namespace Server.Game
{
    public abstract class TileContent
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public abstract TileContentType Type { get; }
        public string Name { get; }
        public virtual bool IsObstacle => false;

        protected TileContent(int row, int column, string name)
        {
            Row = row;
            Column = column;
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

        public ObstacleTileContent(int row, int column, string name) : base(row, column, name)
        {
        }
    }

    public class PlayerTileContent : TileContent
    {
        public override TileContentType Type => TileContentType.Character;

        public GamePlayer Player { get; }

        public PlayerTileContent(int row, int column, GamePlayer player) : base(row, column, player.Player.Name)
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

        public FinishTileContent(int row, int column, Game game) : base(row, column, "Finish")
        {
            _game = game;
        }

        public override TileContentType Type => TileContentType.Finish;

        public override void Process()
        {
            if (_game.Players.Any(x => x.Row == Row && x.Column == Column))
            {
                _game.IsFinished = true;
            }
        }
    }
}