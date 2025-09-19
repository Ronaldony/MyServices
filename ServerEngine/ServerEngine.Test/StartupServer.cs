
//#define TEST_LOCAL

namespace ServerEngine.Test
{
    using Akka.Dispatch.SysMsg;
    using GameCore.Datas.Player;
    using ServerEngine.Config.Database;
    using ServerEngine.Config.Strapi;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Network.UDP;
    using ServerEngine.Test.ConfigManager;
    using ServerEngine.Test.Database.DataObject;
    using ServerEngine.Test.Server;
    using System.Diagnostics;

    internal class StartupServer
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ConfigureAsync()
        {
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();

            // Get appsettins.
            var configStrapi = configuration.GetSection("Config_Strapi").Get<Config_Strapi>();

			/// RemoteConfigureService.
			var remoteConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            remoteConfigureService.Initialize(configStrapi);

            var configGame = await remoteConfigureService.GetConfigData<Config_Game>(configStrapi.ContentType, "Devel_Game");
            var mysqlConnections = await remoteConfigureService.GetConfigData<List<Config_MySQLConnection>>(configStrapi.ContentType, "Devel_MySQL");
            var couchbaseConnections = await remoteConfigureService.GetConfigData<List<Config_CouchbaseConnection>>(configStrapi.ContentType, "Devel_Couchbase");

            GameConfigManager.Initialize(configGame, mysqlConnections, couchbaseConnections);

            // MemoryPoolService
            var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // TestServer
            var testServer = _serviceProvider.GetRequiredService<TestServer>();
            testServer.Start<UdpHandler>(9050, "Project_RPG", false, false, 0, 0);    // port, peer address changable, polling interval.

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

            // IDBObjectConn.
            var dbObjectConn = _serviceProvider.GetRequiredService<IDBObjectConn>();
            await dbObjectConn.InitializeAsync();
        }
    }
}
