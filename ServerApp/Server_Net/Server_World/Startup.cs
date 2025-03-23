using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Server_World
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Server_World.Actors.HuntZone;
    using Server_World.Server.World;
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

            // Data objects.
            hostAppBuilder.Services.AddSingleton<SessionInfoObject>();
            hostAppBuilder.Services.AddSingleton<PlayerInfoObject>();
            hostAppBuilder.Services.AddSingleton<PlayInfoObject>();
            hostAppBuilder.Services.AddSingleton<InventoryInfoObject>();

            // Actors.
            hostAppBuilder.Services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("Root"));
            hostAppBuilder.Services.AddSingleton<HuntZoneActorProvider>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();

                var actor = actorSystem.ActorOf<HuntZoneActor>();

                return new HuntZoneActorProvider(actor);
            });

            // servers.
            hostAppBuilder.Services.AddSingleton<WorldServer>();

            // Scoped.
            hostAppBuilder.Services.AddTransient<DBIdentity>();
            hostAppBuilder.Services.AddTransient<DBUser>();
            hostAppBuilder.Services.AddTransient<DBPlayerData>();
        }
    }
}
