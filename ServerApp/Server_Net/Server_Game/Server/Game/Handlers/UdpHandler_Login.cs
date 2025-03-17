namespace ServerEngine.Test.Network
{
    using Framework.Core.Database.DataObject;
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Packet;
    using Server_Game.Core.Utilities;
    using Server_Game.Server.Game;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Test.
    /// </summary>
    public class UdpHandler_Login : UdpHandler
    {
        private readonly GameServer _gameServer;
        private readonly IDataSerializer _dataSerializer;
        private readonly SessionInfoObject _sessionInfoObject;

        public UdpHandler_Login()
            : base(GamePacketCode.Login)
        {
            _gameServer = ServiceUtility.GetService<GameServer>();
            _dataSerializer = ServiceUtility.GetService<IDataSerializer>();
            _sessionInfoObject = ServiceUtility.GetService<SessionInfoObject>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            ThreadPool.QueueUserWorkItem(async param => 
            {
                var packetLogin = _dataSerializer.Deserialize<Packet_Login>(packet.Data);

                // 세션.
                var sessionInfo = await _sessionInfoObject.SelectAsync<SessionInfo>(packetLogin.PId);

                if (sessionInfo == null)
                {
                    _gameServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // 세션 토큰 검사.
                if (false == sessionInfo.AccessToken.Equals(packetLogin.Token))
                {
                    _gameServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                _gameServer.LoginSession(sessionUid);
            });
        }
    }
}
