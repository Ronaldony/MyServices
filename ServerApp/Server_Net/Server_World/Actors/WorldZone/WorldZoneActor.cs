using Akka.Actor;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Interfaces;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Services;
    using Server_World.Core.Utilities;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.World;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor : ReceiveActor
    {
        private readonly ILogger<WorldZoneActor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;

        private readonly WorldServer _worldServer;
        private readonly WorldZoneScheduler_Enemy _schedulerEnemy;

        // World_Group.
        private readonly World_Group _worldGroup;

        /// <summary>
        /// 필드 적 수.
        /// </summary>
        private int _enemyCount;

        public WorldZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<WorldZoneActor>>();

            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();
            _memoryPoolService =  ServiceUtility.GetService<IMemoryPoolService>();
            _collisionService = ServiceUtility.GetService<CollisionService>();

            _worldServer = ServiceUtility.GetService<WorldServer>();
            _schedulerEnemy = new WorldZoneScheduler_Enemy(ServiceUtility.GetServiceProvider());

            // Initialize.
            _worldGroup = Initialize_WorldGroup();

            ///////////////////////////////// Receive.

            // Spawn.
            Receive<World_Player>(player => SpawnPlayer(player));
            Receive<WorldMsg_SpawnEnemy>(msg => SpawnEnemy(msg));

            // Out player.
            Receive<WorldMsg_OutPlayer>(msg => OutPlayer(msg));

            // Update frame.
            Receive<WorldMsg_UpdateFrame>(msg => UpdateFrame(msg));
            Receive<WorldMsg_UpdateMovePattern>(msg => UpdateMovePattern(msg));

            // NetPacket.
            Receive<WorldMsg_NetPacket>(msg =>
            {
                // World player.
                var worldPlayer = GetPlayerOrDefault(msg.Pid);
                if (default == worldPlayer)
                {
                    return;
                }

                switch (msg.PacketCode)
                {
                    case NetPacketCode.CS_MoveStart:
                        MoveStart(worldPlayer, msg.Packet as NetPacketCS_MoveStart);
                        break;
                    case NetPacketCode.CS_MoveEnd:
                        MoveEnd(worldPlayer, msg.Packet as NetPacketCS_MoveEnd);
                        break;
                    case NetPacketCode.CS_Attack:
                        Attack(worldPlayer, msg.Packet as NetPacketCS_Attack);
                        break;
                    case NetPacketCode.CS_KnockBack:
                        KnockBack(worldPlayer, msg.Packet as NetPacketCS_KnockBack);
                        break;
                    case NetPacketCode.CS_Dash:
                        Dash(worldPlayer, msg.Packet as NetPacketCS_Dash);
                        break;
                }
            });

            // Enemy shceduler start.
            _schedulerEnemy.Start();
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new WorldZoneActor());
        }

        /// <summary>
        /// Get play info.
        /// </summary>
        private World_Player GetPlayerOrDefault(string pid)
        {
            return _worldGroup.PlayerDict.GetValueOrDefault(pid);
        }

        /// <summary>
        /// Add player to section.
        /// </summary>
        private bool AddPlayer(string pid, World_Player player, (int sectionX, int sectionY) section)
        {
            // 섹션에 플레이어 추가.
            if (false == _worldGroup.SectionPlayerDict[section].TryAdd(pid, player))
            {
                return false;
            }

            ///////////////////////////////////////////////////////
            // 플레이어 정보 저장.
            if (false == _worldGroup.PlayerDict.ContainsKey(pid))
            {
                // 중복 플레이어 처리.
                _worldGroup.PlayerDict.Add(pid, player);
            }

            _worldGroup.PlayerDict[pid] = player;

            return true;
        }
    }
}
