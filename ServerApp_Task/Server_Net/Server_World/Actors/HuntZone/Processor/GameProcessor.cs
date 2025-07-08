using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Threading.Channels;

namespace Server_World.Actors.HuntZone.Processor
{
    using GameCore.Interfaces;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Services;
    using Server_World.Core.Utilities;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.World;

    /// <summary>
    /// GameProcessor.
    /// </summary>
    public partial class GameProcessor
    {
        private readonly ILogger<GameProcessor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;

        // Channel - GameEvent.
        private readonly Channel<GameEvent> _gameEventChannel;
        private readonly ChannelWriter<GameEvent> _gameEventWriter;

        // Channel - WorldMsg.
        private readonly Channel<WorldMsg> _worldMsgChannel;
        private readonly ChannelWriter<WorldMsg> _worldMsgWriter;

        // Server.
        private readonly WorldServer _worldServer;

        // WorldGroup.
        private readonly World_Group _worldGroup;

        public GameProcessor(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<GameProcessor>>();

            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();
            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _collisionService = ServiceUtility.GetService<CollisionService>();

            _worldServer = ServiceUtility.GetService<WorldServer>();

            // channels.
            _gameEventChannel = Channel.CreateUnbounded<GameEvent>();
            _gameEventWriter = _gameEventChannel.Writer;

            _worldMsgChannel = Channel.CreateUnbounded<WorldMsg>();
            _worldMsgWriter = _worldMsgChannel.Writer;

            // Process Async.
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
        /// Process async.
        /// </summary>
        private async Task ProcessAsync(CancellationToken ct)
        {
            // reader.
            var gameEventReader = _gameEventChannel.Reader;
            var worldMsgReader = _worldMsgChannel.Reader;

            while (!ct.IsCancellationRequested)
            {
                // Process GameEvent.
                await foreach (var gameEvent in gameEventReader.ReadAllAsync())
                {
                    ProcessGameEvent(gameEvent);
                }

                // Process WorldMsg.
                await foreach (var item in worldMsgReader.ReadAllAsync())
                {

                }

                await Task.Delay(5);
            }
        }
    }
}
