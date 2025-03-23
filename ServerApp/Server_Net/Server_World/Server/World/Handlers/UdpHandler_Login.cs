using System.Reflection;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.Attributes;
    using Framework.Core.Database.DataObject;
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone;
    using Server_World.Actors.HuntZone.Message;
    using Server_World.Core.Utilities;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Login.
    /// </summary>
    public class UdpHandler_Login : UdpHandler
    {
        private readonly WorldServer _worldServer;
        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        // MemoryPoolService.
        private readonly IMemoryPoolService _memoryPoolService;

        /// <summary>
        /// Data object dictionary.
        /// key: Data object type.
        /// value: Data Object.
        /// </summary>
        private readonly Dictionary<Type, IDataObject> _dataObjectDict;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        public UdpHandler_Login()
            : base(GamePacketCode.CS_Login)
        {
            _worldServer = ServiceUtility.GetService<WorldServer>();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();

            // Session object.
            _sessionInfoObject = ServiceUtility.GetService<SessionInfoObject>();

            // MemoryPoolService.
            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();

            // Data object 생성.
            _dataObjectDict = new Dictionary<Type, IDataObject>();

            // InfoObjectAttribute를 포함한 DataObject 가져오기.
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(d => d.GetCustomAttribute<DataInfoObjectAttribute>() != null))
            {
                var dataObject = ServiceUtility.GetService(type) as IDataObject;

                _dataObjectDict.Add(type, dataObject);
            }

            // Actor 생성.
            _huntZoneActorProvider = ServiceUtility.GetService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            ThreadPool.QueueUserWorkItem(async param => 
            {
                var packetLogin = _dataSerializer.Deserialize<NetPacketCS_Login>(packet.Data);

                // 세션.
                var sessionInfo = await _sessionInfoObject.SelectAsync<SessionInfo>(packetLogin.PId);

                if (sessionInfo == null)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // 세션 토큰 검사.
                if (false == sessionInfo.AccessToken.Equals(packetLogin.Token))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Pid 등록.
                if (false == _worldServer.RegisterPid(sessionUid, packetLogin.PId))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                }

                // Load infos.
                if (false == await LoadPlayerToHuntZoneAsync(packetLogin.PId))
                {
                    return;
                }
            });
        }

        /// <summary>
        /// Load player to hunt zone actor.
        /// </summary>
        private async Task<bool> LoadPlayerToHuntZoneAsync(string pid)
        {
            // PlayInfo 로드.
            var playInfoObject = _dataObjectDict[typeof(PlayInfoObject)];
            var playInfo = await playInfoObject.SelectAsync<DTO_PlayInfo>(pid);

            if (null == playInfo)
            {
                return false;
            }

            // Actor msg.
            var actorMsg = _memoryPoolService.Rent<HuntZoneMsg_LoadPlayer>();

            actorMsg.PlayInfo = playInfo;

            // Delivery msg.
            _huntZoneActorProvider.Tell<HuntZoneMsg_LoadPlayer>(actorMsg);

            return true;
        }
    }
}
