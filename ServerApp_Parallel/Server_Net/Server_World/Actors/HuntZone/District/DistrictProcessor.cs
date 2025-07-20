using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Router;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.GameEvents;
    using Server_World.Core.Services;
    using Server_World.Core.Types;
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
        private readonly ProjectileService _projectileService;

        // Server.
        private readonly HuntZoneServer _huntZoneServer;

        // WorldGroup.
        private readonly World_Group _worldGroup;

        // WorldProjectile.
        private readonly World_Projectile _worldProjectile;

        // HuntZoneRouter.
        private readonly HuntZoneRouter _router;

        // District.
        private readonly District _district;

        // Queue.
        private readonly ConcurrentQueue<GameEvent> _gameEventQueue;
        private readonly ConcurrentQueue<WorldMsg> _worldMsgQueue;

        // Aciton block.
        private readonly ActionBlock<Type_WorldProcess> _processPipeline;

        // Scheduler.
        private readonly HuntZoneScheduler_MovePattern _schedulerMovePattern;
        private readonly HuntZoneScheduler_UpdateFrame _schedulerUpdateFrame;

        // Frame time.
        private DateTime _lastFrameUpdateTime;

        public DistrictProcessor(IServiceProvider serviceProvider, HuntZoneRouter router, HashSet<SectionId> aroundSections, District district, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DistrictProcessor>>();

            // service.
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _collisionService = serviceProvider.GetRequiredService<CollisionService>();
            _projectileService = serviceProvider.GetRequiredService<ProjectileService>();

            // Worlds.
            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();
            _worldGroup = Initialize_WorldGroup(aroundSections);
            _worldProjectile = new World_Projectile();

            // Router.
            _router = router;

            // District.
            _district = district;

            // queue.
            _gameEventQueue = new ConcurrentQueue<GameEvent>();
            _worldMsgQueue = new ConcurrentQueue<WorldMsg>();

            // Frame time.
            _lastFrameUpdateTime = TimeUtil.Now;

            // Process pipeline.
            _processPipeline = new ActionBlock<Type_WorldProcess>(
                action: worldProcess => Process(worldProcess)
                , new ExecutionDataflowBlockOptions
                {
                    CancellationToken = ct,
                    MaxDegreeOfParallelism = 1,
                });

            // Schedulers.
            _schedulerMovePattern = new HuntZoneScheduler_MovePattern(serviceProvider, this);
            _schedulerMovePattern.Start();

            _schedulerUpdateFrame = new HuntZoneScheduler_UpdateFrame(serviceProvider, this);
            _schedulerUpdateFrame.Start();

            _logger.LogInformation($"start DistrictProcessor. district: {_district}");
        }

        /// <summary>
        /// Write game event.
        /// </summary>
        public void WriteGameEvent(GameEvent ge)
        {
            _gameEventQueue.Enqueue(ge);
            _processPipeline.Post(Type_WorldProcess.GameEvent);
        }

        /// <summary>
        /// Write world msg.
        /// </summary>
        public void WriteWorldMsg(WorldMsg wm)
        {
            _worldMsgQueue.Enqueue(wm);
            _processPipeline.Post(Type_WorldProcess.WorldMsg);
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
        /// Process.
        /// </summary>
        private void Process(Type_WorldProcess worldProcess)
        {
            switch (worldProcess)
            {
                case Type_WorldProcess.GameEvent:
                    _gameEventQueue.TryDequeue(out var gameEvent);
                    ProcessGameEvent(gameEvent);
                    break;
                case Type_WorldProcess.WorldMsg:
                    _worldMsgQueue.TryDequeue(out var worldMsg);
                    ProcessWorldMsg(worldMsg);
                    break;
                default:
                    _logger.LogError($"process type error. type: {worldProcess}");
                    break;
            }
        }
    }
}
