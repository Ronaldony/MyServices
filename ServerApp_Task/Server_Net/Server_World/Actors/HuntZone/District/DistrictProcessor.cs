using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Diagnostics;
using System.Threading.Channels;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Router;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Services;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.World;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        private readonly ILogger<DistrictProcessor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;

        // Server.
        private readonly WorldServer _worldServer;

        // WorldGroup.
        private readonly World_Group _worldGroup;

        // HuntZoneRouter.
        private readonly HuntZoneRouter _router;

        // District.
        private readonly District _district;

        // Channel - GameEvent.
        private readonly Channel<GameEvent> _gameEventChannel;
        private readonly ChannelWriter<GameEvent> _gameEventWriter;

        // Channel - WorldMsg.
        private readonly Channel<WorldMsg> _worldMsgChannel;
        private readonly ChannelWriter<WorldMsg> _worldMsgWriter;

        public DistrictProcessor(IServiceProvider serviceProvider, HuntZoneRouter router, HashSet<SectionId> aroundSections, District district, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DistrictProcessor>>();

            // service.
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _collisionService = serviceProvider.GetRequiredService<CollisionService>();

            // Worlds.
            _worldServer = serviceProvider.GetRequiredService<WorldServer>();
            _worldGroup = Initialize_WorldGroup(aroundSections);

            // Router.
            _router = router;

            // District.
            _district = district;

            // channels.
            _gameEventChannel = Channel.CreateUnbounded<GameEvent>();
            _gameEventWriter = _gameEventChannel.Writer;

            _worldMsgChannel = Channel.CreateUnbounded<WorldMsg>();
            _worldMsgWriter = _worldMsgChannel.Writer;

            // Process.
            Task.Run(() => ProcessAsync(ct));
        }

        /// <summary>
        /// Write game event.
        /// </summary>
        public bool WriteGameEvent(GameEvent ge)
        {
            return _gameEventWriter.TryWrite(ge);
        }

        /// <summary>
        /// Write world msg.
        /// </summary>
        public bool WriteWorldMsg(WorldMsg wm)
        {
            return _worldMsgWriter.TryWrite(wm);
        }
        
        /// <summary>
        /// Route game event to router.
        /// </summary>
        public void RouteGameEvent<T>(T gameEvent)
            where T : GameEvent
        {
            _router.RouteGameEvent(gameEvent);
        }

        /// <summary>
        /// Process async.
        /// </summary>
        private async Task ProcessAsync(CancellationToken ct)
        {
            // reader.
            var gameEventReader = _gameEventChannel.Reader;
            var worldMsgReader = _worldMsgChannel.Reader;

            int processUpdateTime = 15;
            var sw = new Stopwatch();

            while (!ct.IsCancellationRequested)
            {
                // Start measure.
                sw.Restart();

                // Process GameEvent.
                await foreach (var gameEvent in gameEventReader.ReadAllAsync())
                {
                    // from other GameProcessor.
                    ProcessGameEvent(gameEvent);
                }

                // Process WorldMsg.
                await foreach (var worldMsg in worldMsgReader.ReadAllAsync())
                {
                    ProcessWorldMsg(worldMsg);
                }

                // Stop measure.
                sw.Stop();

                var delayTime = processUpdateTime - (int)sw.ElapsedMilliseconds;
                if (delayTime < 0)
                {
                    delayTime = 0;
                }

                await Task.Delay(delayTime);
            }
        }
    }
}
