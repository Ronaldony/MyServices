using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.GeoIP;
using ServerEngine.MessageBus.RabbitMQs;

namespace Server_Realm
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.Actors;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Actors;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Services;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Zones.Providers;
    using ServerEngine.Database.DBObject.Connections;
    using ServerEngine.Database.Interfaces;

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

            // Connections.
            appBuilder.Services.AddSingleton<RBMQConnection>();
            appBuilder.Services.AddSingleton<IDBObjectConn, CouchbaseConnection>(provider => new CouchbaseConnection(GameConfigManager.ConfigGame.Couchbase));

            appBuilder.Services.AddSingleton<CollisionFactory>();
            appBuilder.Services.AddSingleton<ProjectileFactory>();
            appBuilder.Services.AddSingleton<UnitFactory>();

            appBuilder.Services.AddSingleton<MobCreateService>();
            appBuilder.Services.AddSingleton<ItemCreateService>();

            // Actors.
            appBuilder.Services.AddSingleton<ActorSystem>(service => ActorSystem.Create("root"));
            appBuilder.Services.AddSingleton<HuntZoneProcessorProvider>();
            appBuilder.Services.AddSingleton<HubActorProvider>();
            appBuilder.Services.AddSingleton<DataInfoSaveActorProvider>();

            // Infos.
            appBuilder.Services.AddTransient<DBInventoryInfo>();
            appBuilder.Services.AddTransient<DBPlayerInfo>();
            appBuilder.Services.AddTransient<DBPlayInfo>();
            appBuilder.Services.AddTransient<DBSessionInfo>();
            appBuilder.Services.AddTransient<DBPostInfo>();

            appBuilder.Services.AddSingleton<DBIdentity>();
            appBuilder.Services.AddSingleton<DBUser>();
            appBuilder.Services.AddSingleton<DBPlayerData>();
            appBuilder.Services.AddSingleton<DBPost>();

            // servers.
            appBuilder.Services.AddSingleton<HuntZoneServer>();

            // Transient.
            appBuilder.Services.AddTransient<SessionInfoObject>();
            appBuilder.Services.AddTransient<PlayerInfoObject>();
            appBuilder.Services.AddTransient<PlayInfoObject>();
            appBuilder.Services.AddTransient<InventoryInfoObject>();
            appBuilder.Services.AddTransient<PostInfoObject>();
        }
    }
}
