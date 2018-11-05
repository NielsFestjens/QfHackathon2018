using Microsoft.Extensions.DependencyInjection;
using Server.Game;
using Server.Game.Levels;
using Server.Game.Players;
using Server.Game.Spectators;

namespace Server.WebApi.DependencyInjection
{
    public static class AddGameExtensions
    {
        public static void AddGame(this IServiceCollection services)
        {
            services.AddTransient<IPlayerConnector, PlayerConnector>();
            services.AddTransient<ISpectatorConnector, SpectatorConnector>();
            services.AddTransient<IGameEvents, GameEventsPublisher>();
            services.AddSingleton<IPlayerManager, PlayerManager>();
            services.AddTransient<ILevelManager, LevelManager>();
        }
    }
}