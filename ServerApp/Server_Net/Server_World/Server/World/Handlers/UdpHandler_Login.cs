using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.GameObjects;
    using GameCore;
    using GameCore.NetPacket;

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

        // DB Objects.
        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        public UdpHandler_Login(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_Login)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Login>>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Session object.
            _sessionInfoObject = serviceProvider.GetRequiredService<SessionInfoObject>();

            // Data object 생성.
            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();
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
                var pid = netPacket.Pid;

                // 세션.
                await _sessionInfoObject.LoadDataAsync(pid);

                var sessionInfo = _sessionInfoObject.SessionInfo;

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
                if (false == _worldServer.RegisterPid(sessionUid, pid))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfo = await _dbPlayInfo.SelectAsync(pid);
                if (null == playInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dbInventoryInfo.SelectAsync(pid);

                if (null == inventoryInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Send Login packet.
                _worldServer.Send(sessionId, sessionUid, NetPacketCode.SC_Login,
                    _dataSerializer.Serialize(new NetPacketSC_Login
                    {
                        PlayInfo = playInfo,
                        InventoryInfo = inventoryInfo
                    }));
            });
        }
    }
}
