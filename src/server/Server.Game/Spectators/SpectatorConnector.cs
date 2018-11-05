using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Server.Game.Spectators
{
    public interface ISpectatorConnector
    {
        bool IsConnected(string id);
        bool IsAuthorized(Guid apiKey);
        void Connect(string id);
        void Disconnect(string id);
    }

    public class SpectatorConnector : ISpectatorConnector
    {
        private readonly IConfiguration _configuration;

        public SpectatorConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsConnected(string id)
        {
            return true;
        }

        public bool IsAuthorized(Guid apiKey)
        {
            var apiKeys = _configuration.GetSection("Game:Spectators:ApiKeys").Get<string[]>();
            return apiKeys.Contains(apiKey.ToString());
        }

        public void Connect(string id) { }

        public void Disconnect(string id) { }
    }
}