
//#define TEST_LOCAL

namespace ServerEngine.Test
{
    using GameCore.Datas.Player;
    using ServerEngine.Config;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Config.Database;
    using ServerEngine.Config.Strapi;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Test.Database.DataObject;
    using ServerEngine.Test.Server;

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

			// MemoryPoolService
			var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // TestServer
            var testServer = _serviceProvider.GetRequiredService<TestServer>();
            testServer.Start(9050, false);    // port, peer address changable, polling interval.

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

#if! TEST_LOCAL
            var configGame = await remoteConfigureService.GetConfigData<Config_Game>("Devel_Game");
            var mysqlConnections = await remoteConfigureService.GetConfigData<List<Config_MySQLConnection>>("Devel_MySQL");
            var couchbaseConnections = await remoteConfigureService.GetConfigData<List<Config_CouchbaseConnection>>("Devel_Couchbase");

            GameConfigManager.Initialize(configGame, mysqlConnections, couchbaseConnections);

            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(configGame.SnowflakeBaseTime, 1, 2);
#endif

            var playerInfoObject = _serviceProvider.GetRequiredService<PlayerInfoObejct>();
            await playerInfoObject.UpsertAsync<DTO_PlayerInfo>("0000018CFDF36A52", new DTO_PlayerInfo
            {
                Pid  = "0000018CFDF36A52",
                RegTime = DateTime.Now,
                PlayerName = "0000018CFDF36A52",
                Uid = "0000018CFDF36A52"
            });
        }
    }
}
