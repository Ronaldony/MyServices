using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Server_Game
{
    using Akka.Actor;
    using Framework.Core.Database.DAO;
    using Server_Game.Actors.HuntZone;
    using Server_Game.Server.Game;
    using ServerEngine.Core.Services;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.GeoIP;

    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        public void Configure(IHostApplicationBuilder hostAppBuilder)
        {
            // logging.
            hostAppBuilder.Logging.ClearProviders();
            hostAppBuilder.Logging.AddNLog();
            hostAppBuilder.Logging.SetMinimumLevel(LogLevel.Trace);

            // services.
            hostAppBuilder.Services.AddSingleton<IRemoteConfigureService, StrapiConfigureService>();
            hostAppBuilder.Services.AddSingleton<IUniqueIdService, SnowflakeService>();
            hostAppBuilder.Services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
            hostAppBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();

            hostAppBuilder.Services.AddSingleton<IMemoryPoolService, CustomMemoryPoolService>();
            hostAppBuilder.Services.AddSingleton<IGeoIPService, MaxMindIPService>();

            // Actors.
            hostAppBuilder.Services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("Root"));
            hostAppBuilder.Services.AddSingleton<HuntZoneActor>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();

                var actor = actorSystem.ActorOf(Props.Create(() => new HuntZoneActor()));

                return () => actor;
            });

            // servers.
            hostAppBuilder.Services.AddSingleton<GameServer>();

            // Scoped.
            hostAppBuilder.Services.AddTransient<DBIdentity>();
            hostAppBuilder.Services.AddTransient<DBUser>();
            hostAppBuilder.Services.AddTransient<DBPlayerData>();
        }
    }
}
