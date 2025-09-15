using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;
using System.Reactive.Linq;

namespace Server_Realm.Zones.HuntZone
{
    using Framework.Core.Database.Actors;
    using Framework.Core.Utilities;
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Actors;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.TaskSchedulers;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Worlds.MsgTrigger;
    using Server_Realm.Zones;
    using Server_Realm.Zones.Providers;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor : ReceiveActor, IZoneProcessor
    {
        private readonly ILogger<HuntZoneProcessor> _logger;

        // Services.
        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;

        // Factories.
        private readonly UnitFactory _unitFactory;

        // Servers.
        private readonly HuntZoneServer _zoneServer;

        // Providers.
        private readonly HuntZoneProcessorProvider _zoneProvider;
        private readonly HubActorProvider _hubActorProvider;
        private readonly DataInfoSaveActorProvider _dataInfoSaveProvider;

        // Worlds.
        private readonly WorldGroup _worldGroup;

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

        public HuntZoneProcessor(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneProcessor>>();

            // service.
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            // Factories.
            _unitFactory = serviceProvider.GetRequiredService<UnitFactory>();

            // Servers.
            _zoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            // Providers.
            _zoneProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
            _hubActorProvider = serviceProvider.GetRequiredService<HubActorProvider>();
            _dataInfoSaveProvider = serviceProvider.GetRequiredService<DataInfoSaveActorProvider>();

            // Worlds.
            _worldGroup = Initialize_WorldGroup(serviceProvider, this);

            // Frame time.
            _lastFrameUpdateTime = TimeUtil.Now;

            _cts = ServiceUtility.GetCTS();

            // Task nodes.
            _taskDispatcher = new TaskDispatcher_WorldSection(serviceProvider, _cts.Token);
            _taskDispatcher.Initialize(_worldGroup.AroundSectionDict.Keys, UpdateFrame);

            // Add task node types.
            _taskNodeTypes = new List<Type_TaskNode>();
            _taskNodeTypes.Add(Type_TaskNode.AI);
            _taskNodeTypes.Add(Type_TaskNode.Combat);

            // Register receiver.
            RegisterReceiver();

            // Message trigger.
            _msgTrgSpawnMob = new WorldMsgTrigger_SpawnMob(serviceProvider, this);
            _msgTrgSpawnMob.Start(Type_Area.HuntZone);

            // Scheduler - update frame.
            Observable
                .Interval(TimeSpan.FromSeconds(0.033))    // 33FPS
                .Subscribe(count =>
                {
                    var msg = _memoryPoolService.Rent<WorldMsg_UpdateFrame>();
                    Tell(msg);
                });

            // Scheduler - save datas.
            Observable
                .Interval(TimeSpan.FromSeconds(3))    // 33FPS
                .Subscribe(count =>
                {
                    Tell("save_datas");
                });
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props(IServiceProvider serviceProvider)
        {
            return Akka.Actor.Props.Create(() => new HuntZoneProcessor(serviceProvider));
        }

        /// <summary>
        /// Self tell.
        /// </summary>
        public void Tell<T>(T msg)
        {
            _zoneProvider.Tell(msg);
        }

        /// <summary>
        /// Process.
        /// </summary>
        private void RegisterReceiver()
        {
            // msg.
            Receive<string>(msg =>
            {
                switch (msg)
                {
                    case "save_datas":
                        SaveDatas();
                        break;
                }
            });

            // Update frame.
            ReceiveAsync<WorldMsg_UpdateFrame>(async msg =>
            {
                await DispatchUpdateFrameAsync();
                _memoryPoolService.Release(msg);
            });

            // Spawn.
            Receive<WorldMsg_SpawnMob>(wm => SpawnMobs(wm));
            Receive<WorldMsg_EnterPlayer>(wm => EnterPlayer(wm));
            Receive<WorldMsg_LeavePlayer>(wm => LeavePlayer(wm));

            // Update.
            Receive<WorldMsg_UpdateDummy>(wm => UpdateDummy(wm));

            // Post.
            Receive<WorldMsg_ClaimPost>(wm => ClaimPost(wm));

            // GamePacket.
            Receive<WorldMsg_GamePacket>(wm =>
            {
                // World player.
                if (false == _worldGroup.PlayerUnitDict.ContainsKey(wm.Pid))
                {
                    return;
                }

                // Get pu.
                var pu = _worldGroup.PlayerUnitDict[wm.Pid];

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
                    case GamePacketCode.CS_RootItem:
                        RootItem(pu, wm.Packet as GamePacketCS_RootItem);
                        break;
                    case GamePacketCode.CS_SwitchWeapon:
                        SwitchWeapon(pu, wm.Packet as GamePacketCS_SwitchWeapon);
                        break;
                }
            });
        }

        /// <summary>
        /// Respawn mob.
        /// </summary>
        public void RespawnMob(int areaSpawnId)
        {
            _msgTrgSpawnMob.RequestRespawn(areaSpawnId);
        }
    }
}
