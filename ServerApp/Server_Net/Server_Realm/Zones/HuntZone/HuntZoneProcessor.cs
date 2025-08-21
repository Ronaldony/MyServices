using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;
using System.Reactive.Linq;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Core.Utilities;
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

        // Services.
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;

        // Factories.
        private readonly UnitFactory _unitFactory;

        // Zones.
        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneProcessorProvider _zoneProcessorProvider;

        // Worlds.
        private readonly WorldGroup _worldGroup;
        private readonly WorldUnitCollector _worldUnitCollector;

        // TaskDispatcher.
        private readonly TaskDispatcher_WorldSection _taskDispatcher;
        private List<Type_TaskNode> _taskNodeTypes;

        // Frame time.
        private DateTime _lastFrameUpdateTime;
        private double _frameDeltaSec;

        // Cancel token.
        private CancellationTokenSource _cts;

        // Scheduler spawn mob.
        private readonly WorldMsgTrigger_SpawnMob _msgTrgSpawnMob;

        // Observers.
        private readonly IDisposable _updateFrameObserver;

        private int _benchFrameCount;
        private double _benchFrameSum;

        public HuntZoneProcessor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneProcessor>>();

            // service.
            _serviceProvider = ServiceUtility.GetServiceProvider();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();

            // Factories.
            _unitFactory = ServiceUtility.GetService<UnitFactory>();

            // Zones.
            _huntZoneServer = ServiceUtility.GetService<HuntZoneServer>();
            _zoneProcessorProvider = ServiceUtility.GetService<HuntZoneProcessorProvider>();

            // Worlds.
            _worldGroup = Initialize_WorldGroup(_serviceProvider, this);
            _worldUnitCollector = new WorldUnitCollector();

            // Frame time.
            _lastFrameUpdateTime = TimeUtil.Now;

            _cts = new CancellationTokenSource();

            // Task nodes.
            _taskDispatcher = new TaskDispatcher_WorldSection(_cts.Token);
            _taskDispatcher.Initialize(_worldGroup.AroundSectionDict.Keys, UpdateFrame);

            _taskNodeTypes = new List<Type_TaskNode>();
            _taskNodeTypes.Add(Type_TaskNode.AI);
            _taskNodeTypes.Add(Type_TaskNode.Combat);

            // Register receiver.
            RegisterReceiver();

            // Update frame trigger - 30FPS.
            _updateFrameObserver = Observable.Interval(TimeSpan.FromSeconds(0.033))
                                            .Subscribe(count =>
                                            {
                                                Tell(new WorldMsg_UpdateFrame());
                                            });

            // Message trigger.
            _msgTrgSpawnMob = new WorldMsgTrigger_SpawnMob(_serviceProvider, this);
            _msgTrgSpawnMob.Start();
        }

        /// <summary>
        /// Self tell.
        /// </summary>
        public void Tell<T>(T msg)
        {
            _zoneProcessorProvider.Tell(msg);
        }

        /// <summary>
        /// Process.
        /// </summary>
        private void RegisterReceiver()
        {
            // Update frame.
            ReceiveAsync<WorldMsg_UpdateFrame>(async wm => await DispatchUpdateFrameAsync(wm));

            // Spawn.
            Receive<WorldMsg_SpawnMob>(wm => SpawnMobs(wm));
            Receive<WorldMsg_SpawnUnits>(wm => SpawnPlayer(wm));
            Receive<WorldMsg_LeavePlayer>(wm => LeavePlayer(wm));

            // Update.
            Receive<WorldMsg_UpdateDummy>(wm => UpdateDummy(wm));

            // GamePacket.
            Receive<WorldMsg_GamePacket>(wm =>
            {
                // World player.
                var pu = _worldGroup.PlayerUnitDict.GetValueOrDefault(wm.Pid);
                if (default == pu)
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
                    case GamePacketCode.CS_SwitchWeapon:
                        SwitchWeapon(pu, wm.Packet as GamePacketCS_SwitchWeapon);
                        break;
                }
            });
        }
    }
}
