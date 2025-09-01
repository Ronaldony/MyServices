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
    using Server_Realm.Actors;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Services;
    using Server_Realm.Hubs;
    using Server_Realm.Server.Hub;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Zones;

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

            // zones.
            appBuilder.Services.AddSingleton<ZoneRouter>();

            // Actors.
            appBuilder.Services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("Root"));
            appBuilder.Services.AddSingleton<HuntZoneProcessorProvider>();
            appBuilder.Services.AddSingleton<PlayHubProvider>();
            appBuilder.Services.AddSingleton<PushPostActorProvider>();

            // Infos.
            appBuilder.Services.AddSingleton<DBInventoryInfo>();
            appBuilder.Services.AddSingleton<DBPlayerInfo>();
            appBuilder.Services.AddSingleton<DBPlayInfo>();
            appBuilder.Services.AddSingleton<DBSessionInfo>();
            appBuilder.Services.AddSingleton<DBPostInfo>();

            appBuilder.Services.AddSingleton<DBIdentity>();
            appBuilder.Services.AddSingleton<DBUser>();
            appBuilder.Services.AddSingleton<DBPlayerData>();
            appBuilder.Services.AddSingleton<DBPost>();

            // servers.
            appBuilder.Services.AddSingleton<HuntZoneServer>();
            appBuilder.Services.AddSingleton<HubServer>();

            // Transient.
            appBuilder.Services.AddTransient<SessionInfoObject>();
            appBuilder.Services.AddTransient<PlayerInfoObject>();
            appBuilder.Services.AddTransient<PlayInfoObject>();
            appBuilder.Services.AddTransient<InventoryInfoObject>();
        }
    }
}
