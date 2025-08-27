using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.GeoIP;

namespace Server_PlayHub
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.Interfaces;

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

            // Actors.
            appBuilder.Services.AddSingleton<ActorSystem>(provider => ActorSystem.Create("Root"));

            // Infos.
            appBuilder.Services.AddSingleton<DBInventoryInfo>();
            appBuilder.Services.AddSingleton<DBPlayerInfo>();
            appBuilder.Services.AddSingleton<DBPlayInfo>();
            appBuilder.Services.AddSingleton<DBSessionInfo>();
            appBuilder.Services.AddSingleton<DBPostInfo>();

            appBuilder.Services.AddSingleton<DBIdentity>();
            appBuilder.Services.AddSingleton<DBUser>();
            appBuilder.Services.AddSingleton<DBPlayerData>();

            // Transient.
            appBuilder.Services.AddTransient<SessionInfoObject>();
        }
    }
}