using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using Framwork.Core.Session;
    using GameCore.Datas;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
    using Microsoft.AspNetCore.Authentication;
    using Server_World.Actors.HuntZone.Message;
    using Server_World.Core.Utilities;
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

        private readonly WorldServer _worldServer;

        /// <summary>
        /// GameSession dictionary.
        /// key: Pid.
        /// value: GameSession.
        /// </summary>
        private readonly Dictionary<string, GameSession> _sessionDict;

        /// <summary>
        /// DTO_InfoBase dictionary.
        /// key: Pid.
        /// value: DTO_PlayInfo.
        /// </summarys
        private readonly Dictionary<string, DTO_PlayInfo> _playInfoDict;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();

            _worldServer = ServiceUtility.GetService<WorldServer>();

            _sessionDict = new Dictionary<string, GameSession>();
            _playInfoDict = new Dictionary<string, DTO_PlayInfo>();

            ///////////////////////////////// Receive.

            // Messages.
            Receive<HuntZoneMsg_LoadPlayer>(packet =>
            {
                LoadPlayer(packet);

                _memoryPoolService.Release(packet);
            });

            // Move.
            Receive<NetPacketCS_MoveStart>(packet => MoveStart(packet));
            Receive<NetPacketCS_MoveEnd>(packet => MoveEnd(packet));

            // Interaction.
            Receive<NetPacketCS_Attack>(packet => Attack(packet));
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new HuntZoneActor());
        }
    }
}
