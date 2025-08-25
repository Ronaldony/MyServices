using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.GeoIP;

namespace Server_Realm
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Services;
    using Server_Realm.Server.HuntZone;

    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        public void Configure(IHostApplicationBuilder appBuilder)
        {
            // logging.
            appBuilder.Logging.ClearProviders();
            appBuilder.Logging.AddNLog();
            appBuilder.Logging.SetMinimumLevel(LogLevel.Trace);

            // services.
            appBuilder.Services.AddSingleton<IMemoryPoolService, CustomMemoryPoolService>();
            appBuilder.Services.AddSingleton<IGeoIPService, MaxMindIPService>();
            appBuilder.Services.AddSingleton<IRemoteConfigureService, StrapiConfigureService>();
            appBuilder.Services.AddSingleton<IUniqueIdService, SnowflakeService>();
            appBuilder.Services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
            appBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
            appBuilder.Services.AddSingleton<ITDBService, TDBService>();
            
            appBuilder.Services.AddSingleton<CollisionFactory>();
            appBuilder.Services.AddSingleton<ProjectileFactory>();
            appBuilder.Services.AddSingleton<UnitFactory>();

            appBuilder.Services.AddSingleton<MobCreateService>();
            appBuilder.Services.AddSingleton<ItemCreateService>();

            // Actors.
            appBuilder.Services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("Root"));
            appBuilder.Services.AddSingleton<HuntZoneProcessorProvider>();

            appBuilder.Services.AddSingleton<DBInventoryInfo>();
            appBuilder.Services.AddSingleton<DBPlayerInfo>();
            appBuilder.Services.AddSingleton<DBPlayInfo>();
            appBuilder.Services.AddSingleton<DBSessionInfo>();

            appBuilder.Services.AddSingleton<DBIdentity>();
            appBuilder.Services.AddSingleton<DBUser>();
            appBuilder.Services.AddSingleton<DBPlayerData>();
            
            // servers.
            appBuilder.Services.AddSingleton<HuntZoneServer>();

            // Transient.
            appBuilder.Services.AddTransient<SessionInfoObject>();
            appBuilder.Services.AddTransient<PlayerInfoObject>();
            appBuilder.Services.AddTransient<PlayInfoObject>();
            appBuilder.Services.AddTransient<InventoryInfoObject>();
        }
    }
}
