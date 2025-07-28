using Akka.Actor;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using GameCore.Interfaces;
    using Server_Realm.Core.Services;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Entities;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.TaskSchedulers;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Worlds.MsgTrigger;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor : ReceiveActor, IZoneProcessor
    {
        private readonly ILogger<HuntZoneProcessor> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;
        private readonly ProjectileService _projectileService;

        // Server.
        private readonly HuntZoneServer _huntZoneServer;

        // WorldGroup. 
        private readonly World_Group _worldGroup;

        // TaskDispatcher.
        private readonly TaskDispatcher_WorldSection _taskDispatcher;

        // Frame time.
        private DateTime _lastFrameUpdateTime;
        private double _frameDeltaSec;

        // Cancel token.
        private CancellationTokenSource _cts;

        // Scheduler spawn mob.
        private readonly WorldMsgTrigger_SpawnMob _msgTriggerSpawnMob;

        public HuntZoneProcessor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneProcessor>>();

            // service.
            _serviceProvider = ServiceUtility.GetServiceProvider();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();
            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _collisionService = ServiceUtility.GetService<CollisionService>();
            _projectileService = ServiceUtility.GetService<ProjectileService>();

            // Worlds.
            _huntZoneServer = ServiceUtility.GetService<HuntZoneServer>();
            _worldGroup = Initialize_WorldGroup();

            // Shceduler.
            _msgTriggerSpawnMob = new WorldMsgTrigger_SpawnMob(_serviceProvider, this);

            // Frame time.
            _lastFrameUpdateTime = TimeUtil.Now;

            // Dispatcher.
            _taskDispatcher = new TaskDispatcher_WorldSection(_cts.Token);
            _taskDispatcher.Initialize(_worldGroup.AroundSectionDict.Keys, UpdateFrame);

            // Register receiver.
            RegisterReceiver();

            _cts = new CancellationTokenSource();

            // Schedulers.
            _msgTriggerSpawnMob.Start();
        }

        /// <summary>
        /// Self tell.
        /// </summary>
        public void SelfTell<T>(T msg)
        {
            Self.Tell(msg);
        }

        /// <summary>
        /// Process.
        /// </summary>
        private void RegisterReceiver()
        {
            // Update frame.
            ReceiveAsync<WorldMsg_UpdateFrame>(async wm => await DispatchUpdateFrameAsync(wm));

            // Spawn.
            Receive<WorldMsg_SpawnEnemy>(wm => SpawnEnemy(wm));
            Receive<WorldMsg_SpawnPlayer>(wm => SpawnPlayer(wm));
            Receive<WorldMsg_LeavePlayer>(wm => LeavePlayer(wm));

            // Update.
            Receive<WorldMsg_UpdateDummy>(wm => UpdateDummy(wm));

            // GamePacket.
            Receive<WorldMsg_GamePacket>(wm =>
            {
                // World player.
                var unit = _worldGroup.UnitDict.GetValueOrDefault(wm.UnitId);
                if (default == unit)
                {
                    return;
                }

                // Check is PlayerUnit.
                var pu = unit as PlayerUnit;
                if (pu == null)
                {
                    return;
                }

                switch (wm.PacketCode)
                {
                    case GamePacketCode.CS_MoveStart:
                        MoveStart(pu, wm.Packet as GamePacketCS_MoveStart);
                        break;
                    case GamePacketCode.CS_MoveEnd:
                        MoveEnd(pu, wm.Packet as GamePacketCS_MoveEnd);
                        break;
                    case GamePacketCode.CS_AttackMelee:
                        AttackMelee(pu, wm.Packet as GamePacketCS_AttackMelee);
                        break;
                    case GamePacketCode.CS_AttackProjectile:
                        AttackProjctile(pu, wm.Packet as GamePacketCS_AttackProjectile);
                        break;
                    case GamePacketCode.CS_KnockBack:
                        KnockBack(pu, wm.Packet as GamePacketCS_KnockBack);
                        break;
                    case GamePacketCode.CS_Dash:
                        Dash(pu, wm.Packet as GamePacketCS_Dash);
                        break;
                }
            });
        }
    }
}
