using Akka.Actor;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone
{
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Services;
    using Server_World.Core.Utilities;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.World;

    /// <summary>
    /// HuntZone.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        // Services.
        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;

        // Server
        private readonly WorldServer _worldServer;

        // World group.
        private readonly World_Group _worldGroup;

        // World message queue.
        private readonly ConcurrentQueue<WorldMsg> _worldMsgQueue;

        // Schedulers.
        private readonly WorldZoneScheduler_SpawnEnemy _schedulerEnemy;
        private readonly WorldZoneScheduler_MovePattern _schedulerMovePattern;
        private readonly WorldZoneScheduler_UpdateFrame _schedulerUpdateFrame;
        private readonly WorldZoneScheduler_Dummy _schedulerDummy;

        // Cancel token.
        private readonly CancellationTokenSource _cts;

        // Packet groups.
        // key: group number.
        // value: packet group.
        private readonly Dictionary<int, World_PacketGroup> _packetGroups;

        /// <summary>
        /// 필드 적 수.
        /// </summary>
        private int _enemyCount;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            // Services.
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();
            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _collisionService = ServiceUtility.GetService<CollisionService>();

            // Server.
            _worldServer = ServiceUtility.GetService<WorldServer>();

            // World message queue.
            _worldMsgQueue = new ConcurrentQueue<WorldMsg>();

            // Schedulers.
            _schedulerEnemy = new WorldZoneScheduler_SpawnEnemy(ServiceUtility.GetServiceProvider());
            _schedulerMovePattern = new WorldZoneScheduler_MovePattern(ServiceUtility.GetServiceProvider());
            _schedulerUpdateFrame = new WorldZoneScheduler_UpdateFrame(ServiceUtility.GetServiceProvider());
            _schedulerDummy = new WorldZoneScheduler_Dummy(ServiceUtility.GetServiceProvider());

            // schduler start.
            _schedulerEnemy.Start();
            _schedulerMovePattern.Start();
            _schedulerUpdateFrame.Start();
            //_schedulerDummy.Start();

            _cts = new CancellationTokenSource();

            _packetGroups = new Dictionary<int, World_PacketGroup>();

            // initialize actor receive.
            Initialize_ActorReceive();
        }

        /// <summary>
        /// Stop server.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Initialize receiver of actor.
        /// </summary>
        private void Initialize_ActorReceive()
        {
            // Spawn.
            Receive<WorldMsg>(msg =>
            {
                // 월드 메시지 전달.
                _worldMsgQueue.Enqueue(msg);
            });

            // Spawn.
            Receive<WorldMsg_SpawnPlayer>(player => SpawnPlayer(player));
            Receive<WorldMsg_SpawnEnemy>(msg => SpawnEnemy(msg));

            // Out player.
            Receive<WorldMsg_OutPlayer>(msg => OutPlayer(msg));

            // Update frame.
            Receive<WorldMsg_UpdateFrame>(msg => UpdateFrame(msg));
            Receive<WorldMsg_UpdateMovePattern>(msg => UpdateMovePattern(msg));

            // Dummy.
            Receive<WorldMsg_UpdateDummy>(msg => UpdateDummy(msg));

            // NetPacket.
            Receive<World_NetPacket>(netPacket =>
            {
                // World player.
                var worldPlayer = _worldGroup.PlayerDict.GetValueOrDefault(netPacket.Pid);
                if (default == worldPlayer)
                {
                    return;
                }

                // 패킷 전달.
                worldPlayer.NetPacketQueue.Enqueue(netPacket);
            });
        }
    }
}
