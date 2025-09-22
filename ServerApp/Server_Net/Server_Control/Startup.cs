using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Database.DBObject.Connections;
using ServerEngine.Database.Interfaces;
using ServerEngine.GeoIP;
using ServerEngine.MessageBus.RabbitMQs;

namespace Server_Control
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.Actors;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.Interfaces;
    using Server_Control.Actors;
    using Server_Control.Core.Services;
    using Server_Control.Hubs;
    using Server_Control.Server.Hub;

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
            appBuilder.Services.AddSingleton<CouchbaseConnection>(provider => new CouchbaseConnection());

            // Background.
            appBuilder.Services.AddSingleton<PostService>();
            appBuilder.Services.AddHostedService<PostService>(serviceProvider => serviceProvider.GetRequiredService<PostService>());

            // Actors.
            appBuilder.Services.AddSingleton<ActorSystem>(service => ActorSystem.Create("root"));
            appBuilder.Services.AddSingleton<PlayHubProvider>();
            appBuilder.Services.AddSingleton<HubActorProvider>();
            appBuilder.Services.AddSingleton<DataInfoSaveActorProvider>();

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
            appBuilder.Services.AddSingleton<HubServer>();

            // Transient.
            appBuilder.Services.AddTransient<SessionInfoObject>();
            appBuilder.Services.AddTransient<PlayerInfoObject>();
            appBuilder.Services.AddTransient<PlayInfoObject>();
            appBuilder.Services.AddTransient<InventoryInfoObject>();
            appBuilder.Services.AddTransient<PostInfoObject>();
        }
    }
}
