using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;
    using GameCore.Interfaces;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Scheduler;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;
    using Server_World.Server.World;
    using ServerEngine.Core.Services.Interfaces;

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
        /// Section Player dictionary.
        /// key: Section.
        /// value: Player dictionary (key: pid).
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), Dictionary<string, World_Player>> _sectionPlayerDict;

        /// <summary>
        /// Section enemy dictionary.
        /// key: Section.
        /// value: DTO_Enemy dictionary (key: EnemeyUId).
        /// </summary>
        private readonly Dictionary<(int sectionX, int sectionY), Dictionary<ulong, DTO_Enemy>> _sectionEnemyDict;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _tdbService = ServiceUtility.GetService<ITDBService>();

            _confEnemy = _tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));

            _worldServer = ServiceUtility.GetService<WorldServer>();
            _schedulerEnemy = new HuntZoneScheduler_Enemy(ServiceUtility.GetServiceProvider());
            _schedulerEnemy.Start();

            _playerDict = new Dictionary<string, World_Player>();

            _sectionPlayerDict = new Dictionary<(int sectionX, int sectionY), Dictionary<string, World_Player>>();
            _sectionEnemyDict = new Dictionary<(int sectionX, int sectionY), Dictionary<ulong, DTO_Enemy>>();
            Initialize_Sections(ref _sectionPlayerDict, ref _sectionEnemyDict);

            ///////////////////////////////// Receive.

            // Spanws.
            Receive<World_Player>(msg => SpawnPlayer(msg));

            Receive<WorldMsg_SpawnEnemy>(msg =>SpawnEnemy(msg));

            // Move.
            Receive<NetPacketCS_MoveStart>(packet => MoveStart(packet));
            Receive<NetPacketCS_MoveEnd>(packet => MoveEnd(packet));

            // Attack.
            Receive<NetPacketCS_Attack>(packet => Attack(packet));
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
        private DTO_PlayInfo GetPlayInfo(string pid)
        {
            _playerDict.TryGetValue(pid, out var player);

            return player.PlayInfo;
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
    }
}
