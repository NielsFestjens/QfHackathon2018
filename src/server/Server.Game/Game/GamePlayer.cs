using System;
using Server.Game.Players;

namespace Server.Game
{
    public class GamePlayer: IDisposable
    {
        public Player Player { get; }
        public Game Game { get; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int ViewDistance { get; } = 5;
        public TileContent Tile { get; set; }

        public GamePlayer(Player player, Game game)
        {
            Player = player;
            Game = game;
        }

        public void Dispose()
        {
            Tile = null;
        }
    }
}