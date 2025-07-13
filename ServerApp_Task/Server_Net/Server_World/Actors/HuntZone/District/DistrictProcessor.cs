using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Router;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Services;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.HuntZone;

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
        private readonly HuntZoneServer _huntZoneServer;

        // WorldGroup.
        private readonly World_Group _worldGroup;

        // HuntZoneRouter.
        private readonly HuntZoneRouter _router;

        // District.
        private readonly District _district;

        // Queue.
        private readonly ConcurrentQueue<GameEvent> _gameEventQueue;
        private readonly ConcurrentQueue<WorldMsg> _worldMsgQueue;

        // Scheduler.
        private readonly HuntZoneScheduler_MovePattern _schedulerMovePattern;
        
        public DistrictProcessor(IServiceProvider serviceProvider, HuntZoneRouter router, HashSet<SectionId> aroundSections, District district, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DistrictProcessor>>();

            // service.
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _collisionService = serviceProvider.GetRequiredService<CollisionService>();

            // Worlds.
            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();
            _worldGroup = Initialize_WorldGroup(aroundSections);

            // Router.
            _router = router;

            // District.
            _district = district;

            // queue.
            _gameEventQueue = new ConcurrentQueue<GameEvent>();
            _worldMsgQueue = new ConcurrentQueue<WorldMsg>();

            _schedulerMovePattern = new HuntZoneScheduler_MovePattern(serviceProvider, this);
            _schedulerMovePattern.Start();

            // Process.
            Task.Run(() => ProcessAsync(ct));
        }

        /// <summary>
        /// Write game event.
        /// </summary>
        public void WriteGameEvent(GameEvent ge)
        {
            _gameEventQueue.Enqueue(ge);
        }

        /// <summary>
        /// Write world msg.
        /// </summary>
        public void WriteWorldMsg(WorldMsg wm)
        {
            _worldMsgQueue.Enqueue(wm);
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
        /// Vaildate position..
        /// </summary>
        public bool IsValidSection(Vector2 position)
        {
            return _router.IsValidSection(SectionId.GetSection(position));
        }

        /// <summary>
        /// Process async.
        /// </summary>
        private async Task ProcessAsync(CancellationToken ct)
        {
            _logger.LogInformation($"start DistrictProcessor. district: {_district}.");

            ct.Register(() => _logger.LogError($"shutdown DistrictProcessor. district: {_district}."));

            long processUpdateTime = 30; // 30 FPS
            float frameDeltaSeconds = (float)processUpdateTime / 1000; // 0.03 sec
            var sw = new Stopwatch();

            while (!ct.IsCancellationRequested)
            {
                // Start measure.
                sw.Restart();

                // Process GameEvent.
                while (_gameEventQueue.TryDequeue(out var gameEvent))
                {
                    // from other GameProcessor.
                    ProcessGameEvent(gameEvent);
                }

                // Process WorldMsg.
                while (_worldMsgQueue.TryDequeue(out var worldMsg))
                {
                    // from other GameProcessor.
                    ProcessWorldMsg(worldMsg);
                }

                // Update frame.
                UpdateFrame(frameDeltaSeconds);

                // Stop measure.
                sw.Stop();

                var delayTime = processUpdateTime - sw.ElapsedMilliseconds;
                if (delayTime < 0)
                {
                    delayTime = 0;
                }

                await Task.Delay((int)delayTime);
            }
        }
    }
}
