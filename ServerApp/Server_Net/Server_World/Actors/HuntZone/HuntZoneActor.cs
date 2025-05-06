using Akka.Actor;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone
{
    using GameCore.Datas.Enemy;
    using GameCore.Interfaces;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Datas;
    using Server_World.Core.Services;
    using Server_World.Core.Utilities;
    using Server_World.Core.Worlds;
    using Server_World.Server.World;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionService _collisionService;
        private readonly WeaponService _weaponService;

        private readonly WorldServer _worldServer;
        private readonly HuntZoneScheduler_Enemy _schedulerEnemy;

        /// <summary>
        /// HuntZone_Player dictionary.
        /// key: Pid.
        /// value: HuntZone_Player.
        /// </summary>
        private readonly Dictionary<string, World_Player> _playerDict;

        /// <summary>
        /// 섹션 단위 플레이어 정보.
        /// key: Section.
        /// value: Player dictionary (key: pid).
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), ConcurrentDictionary<string, World_Player>> _sectionPlayerDict;

        /// <summary>
        /// 섹션 단위 적 정보.
        /// key: Section.
        /// value: DTO_Enemy dictionary (key: EnemeyUId).
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), ConcurrentDictionary<ulong, DTO_Enemy>> _sectionEnemyDict;

        /// <summary>
        /// 근처 섹션 정보.
        /// key: Section.
        /// value: Around sections.
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), List<(int sectionX, int sectionY)>> _aroundSectionDict;

        /// <summary>
        /// 섹션 정보.
        /// key: Section.
        /// value: Section class.
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), Section> _sectionDict;

        /// <summary>
        /// 필드 적 수.
        /// </summary>
        private int _enemyCount;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();
            _memoryPoolService =  ServiceUtility.GetService<IMemoryPoolService>();
            _collisionService = ServiceUtility.GetService<CollisionService>();
            _weaponService = ServiceUtility.GetService<WeaponService>();

            _worldServer = ServiceUtility.GetService<WorldServer>();
            _schedulerEnemy = new HuntZoneScheduler_Enemy(ServiceUtility.GetServiceProvider());

            _playerDict = new Dictionary<string, World_Player>();

            // initilzie sections.
            (_sectionPlayerDict, _sectionEnemyDict, _aroundSectionDict, _sectionDict) = Initialize_Sections();

            ///////////////////////////////// Receive.

            // Spanw.
            Receive<World_Player>(player => SpawnPlayer(player));
            Receive<WorldMsg_SpawnEnemy>(msg => SpawnEnemy(msg));

            // Out player.
            Receive<WorldMsg_OutPlayer>(msg => OutPlayer(msg));

            // Update frame.
            Receive<WorldMsg_UpdateFrame>(msg => UpdateFrame(msg));
            Receive<WorldMsg_UpdateMovePattern>(msg => UpdateMovePattern(msg));

            // Move.
            Receive<NetPacketCS_MoveStart>(packet => MoveStart(packet));
            Receive<NetPacketCS_MoveEnd>(packet => MoveEnd(packet));

            // Attack.
            Receive<NetPacketCS_Attack>(packet => Attack(packet));

            // KnockBack.
            Receive<NetPacketCS_KnockBack>(packet => KnockBack(packet));
            Receive<NetPacketCS_Dash>(packet => Dash(packet));

            // Enemy shceduler start.
            _schedulerEnemy.Start();
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new HuntZoneActor());
        }

        /// <summary>
        /// Get play info.
        /// </summary>
        private World_Player GetPlayerOrDefault(string pid)
        {
            return _playerDict.GetValueOrDefault(pid);
        }

        /// <summary>
        /// Add player to section.
        /// </summary>
        private bool AddPlayer(string pid, World_Player player, (int sectionX, int sectionY) section)
        {
            // 섹션에 플레이어 추가.
            if (false == _sectionPlayerDict[section].TryAdd(pid, player))
            {
                return false;
            }

            ///////////////////////////////////////////////////////
            // 플레이어 정보 저장.
            if (false == _playerDict.ContainsKey(pid))
            {
                // 중복 플레이어 처리.
                _playerDict.Add(pid, player);
            }

            _playerDict[pid] = player;

            return true;
        }
    }
}
