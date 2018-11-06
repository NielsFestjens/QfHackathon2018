using System.Collections.Generic;

namespace Server.Game.Players
{
    public interface IPlayerManager
    {
        Player CreatePlayer(string id, string name);
        Player GetPlayer(string id);
    }

    public class PlayerManager : IPlayerManager
    {
        public Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public Player CreatePlayer(string id, string name)
        {
            var player = new Player(id, name);
            _players.Add(id, player);
            return player;
        }

        public Player GetPlayer(string id)
        {
            return _players[id];
        }
    }
}