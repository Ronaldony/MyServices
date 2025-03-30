using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.Attributes;
    using Framework.Core.Database.DataObject;
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Login.
    /// </summary>
    public class UdpHandler_Login : UdpHandler
    {
        private readonly ILogger<UdpHandler_Login> _logger;

        private readonly WorldServer _worldServer;
        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        /// <summary>
        /// Data object dictionary.
        /// key: Data object type.
        /// value: Data Object.
        /// </summary>
        private readonly Dictionary<Type, IDataObject> _dataObjectDict;

        public UdpHandler_Login(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_Login)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Login>>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Session object.
            _sessionInfoObject = serviceProvider.GetRequiredService<SessionInfoObject>();

            // Data object 생성.
            _dataObjectDict = new Dictionary<Type, IDataObject>();

            // InfoObjectAttribute를 포함한 DataObject 가져오기.
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes().Where(d => d.GetCustomAttribute<DataInfoObjectAttribute>() != null))
                {
                    var dataObject = serviceProvider.GetRequiredService(type) as IDataObject;

                    _dataObjectDict.Add(type, dataObject);
                }
            }
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            ThreadPool.QueueUserWorkItem(async param => 
            {
                _logger.LogDebug($"Player Login");

                var netPacket = _dataSerializer.Deserialize<NetPacketCS_Login>(packet.Data);

                // 세션.
                var sessionInfo = await _sessionInfoObject.SelectAsync<SessionInfo>(netPacket.Pid);

                if (sessionInfo == null)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // 세션 토큰 검사.
                if (false == sessionInfo.AccessToken.Equals(netPacket.Token))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Pid 등록.
                if (false == _worldServer.RegisterPid(sessionUid, netPacket.Pid))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfo = await _dataObjectDict[typeof(PlayInfoObject)].SelectAsync<DTO_PlayInfo>(netPacket.Pid);

                if (null == playInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dataObjectDict[typeof(InventoryInfoObject)].SelectAsync<DTO_InventoryInfo>(netPacket.Pid);

                if (null == inventoryInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Send Login packet.
                NetPacketUtility.SendPacket<NetPacketSC_Login>(packet =>
                {
                    packet.PlayInfo = playInfo;
                    packet.InventoryInfo = inventoryInfo;

                    _worldServer.Send(sessionId, sessionUid, NetPacketCode.SC_Login, _dataSerializer.Serialize(packet));
                });
            });
        }
    }
}
