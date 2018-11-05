namespace Server.Game.Players
{
    public interface IPlayerConnector
    {
        bool IsConnected(string id);
        void Connect(string id, string name);
        void Disconnect(string id);
    }

    public class PlayerConnector : IPlayerConnector
    {
        private readonly IGameEvents _events;

        public PlayerConnector(IGameEvents events)
        {
            _events = events;
        }

        public bool IsConnected(string id)
        {
            return true;
        }

        public void Connect(string id, string name)
        {
            _events.OnPlayerConnected(id, name);
        }

        public void Disconnect(string id)
        {
            _events.OnPlayerDisconnected(id);
        }
    }
}