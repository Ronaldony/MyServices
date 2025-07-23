
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

            var configGame = await remoteConfigureService.GetConfigData<Config_Game>(configStrapi.ContentType, "Devel_Game");

            // MemoryPoolService
            var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // TestServer
            var testServer = _serviceProvider.GetRequiredService<TestServer>();
            testServer.Start(9050, false);    // port, peer address changable, polling interval.

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();
        }

        /// <summary>
        /// Configuration for Game server.
        /// </summary>
        public class Config_Game
        {
            /// <summary>
            /// Environment for server.
            /// </summary>
            public string Environment { get; set; }

            /// <summary>
            /// Server.
            /// </summary>
            public string Server { get; set; }

            /// <summary>
            /// Service language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// SnowflakeBaseTime.
            /// </summary>
            public DateTime SnowflakeBaseTime { get; set; }

            /// <summary>
            /// Frame rate.
            /// </summary>
            public int FrameRate { get; set; }

            /// <summary>
            /// ConfigCouchbase.
            /// </summary>
            public Config_Couchbase Couchbase { get; set; }
        }
    }
}
