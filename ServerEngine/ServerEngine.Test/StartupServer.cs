
//#define TEST_LOCAL

namespace ServerEngine.Test
{
    using GameCore.Datas.Player;
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

            var playerInfoObject = _serviceProvider.GetRequiredService<PlayerInfoObejct>();
            await playerInfoObject.UpsertAsync<DTOPlayerInfo>("0000018CFDF36A52", new DTOPlayerInfo
            {
                Pid  = "0000018CFDF36A52",
                RegTime = DateTime.Now,
                PlayerName = "0000018CFDF36A52",
                Uid = "0000018CFDF36A52"
            });
        }
    }
}
