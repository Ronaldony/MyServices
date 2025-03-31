using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Interfaces;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;
    using Server_World.Server.World;
    using ServerEngine.Core.Services.Interfaces;
    using System.Collections.Concurrent;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        private readonly IMemoryPoolService _memoryPoolService;
        private readonly IDataSerializer _dataSerializer;
        private readonly ITDBService _tdbService;

        private readonly WorldServer _worldServer;
        private readonly HuntZoneScheduler_Enemy _schedulerEnemy;

        private readonly CONF_Enemy _confEnemy;

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

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();

            _confEnemy = _tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));

            _worldServer = ServiceUtility.GetService<WorldServer>();
            _schedulerEnemy = new HuntZoneScheduler_Enemy(ServiceUtility.GetServiceProvider());

            _playerDict = new Dictionary<string, World_Player>();

            // initilzie sections.
            (_sectionPlayerDict, _sectionEnemyDict, _aroundSectionDict) = Initialize_Sections();

            ///////////////////////////////// Receive.

            // Spanws.
            Receive<World_Player>(msg => SpawnPlayer(msg));

            Receive<WorldMsg_SpawnEnemy>(msg => SpawnEnemy(msg));

            // Update frame.
            Receive<WorldMsg_UpdateFrame>(msg => UpdateFrame(msg));
            Receive<WorldMsg_UpdateMovePattern>(msg => UpdateMovePattern(msg));

            // Move.
            Receive<NetPacketCS_MoveStart>(packet => MoveStart(packet));
            Receive<NetPacketCS_MoveEnd>(packet => MoveEnd(packet));

            // Attack.
            Receive<NetPacketCS_Attack>(packet => Attack(packet));

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
        /// Send packet.
        /// </summary>
        private bool SendPacket(string pid, int packetCode, byte[] packetData)
        {
            if (false == _playerDict.TryGetValue(pid, out var player))
            {
                return false;
            }

            // Send.
            return _worldServer.Send(player.GameSession.SessionId, player.GameSession.SessionUid, packetCode, packetData);
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        private bool SendPacket(int sessionId, ulong sessionuid, int packetCode, byte[] packetData)
        {
            // Send.
            return _worldServer.Send(sessionId, sessionuid, packetCode, packetData);
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        private bool BroadCast(string pid, int packetCode, byte[] packetData)
        {
            if (false == _playerDict.TryGetValue(pid, out var player))
            {
                return false;
            }

            var gameSession = player.GameSession;

            return _worldServer.BroadCast(gameSession.SessionId, gameSession.SessionUid, packetCode, packetData);
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        private bool BroadCast(int sessionId, ulong sessionuid, int packetCode, byte[] packetData)
        {
            return _worldServer.BroadCast(sessionId, sessionuid, packetCode, packetData);
        }

        /// <summary>
        /// Add player to section.
        /// </summary>
        private bool AddPlayerIntoWorld(string pid, World_Player player, int sectionX, int sectionY)
        {
            // 유저 좌표 검사.
            var section = (sectionX, sectionY);

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
