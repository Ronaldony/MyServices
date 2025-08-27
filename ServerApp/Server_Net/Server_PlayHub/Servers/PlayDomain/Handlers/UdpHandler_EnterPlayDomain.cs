using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.PlayDomain.Handlers
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using Framework.Core.Exceptions;
    using GameCore;
    using GameCore.GamePacket;
    using Server_PlayHub.Server.HuntZone;
    using Server_Realm.Server.HuntZone;

    /// <summary>
    /// UdpHandler_EnterPlayDomain.
    /// </summary>
    public class UdpHandler_EnterPlayDomain : UdpHandler
    {
        private readonly ILogger<UdpHandler_EnterPlayDomain> _logger;

        private readonly PlayDomainServer _server;

        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        public UdpHandler_EnterPlayDomain(IServiceProvider serviceProvider)
            : base(DomainPacketCode.CS_EnterPlayDomain)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_EnterPlayDomain>>();

            _server = serviceProvider.GetRequiredService<PlayDomainServer>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Session object.
            _sessionInfoObject = serviceProvider.GetRequiredService<SessionInfoObject>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            Task.Run(async () => 
            {
                _logger.LogDebug($"Player enter into play domain");

                var GamePacket = _dataSerializer.Deserialize<GamePacketCS_Enter>(packet.Data);
                var pid = GamePacket.Pid;

                try
                {
                    // 세션.
                    await _sessionInfoObject.LoadDataAsync(pid);

                    var sessionInfo = _sessionInfoObject.SessionInfo;
                    if (sessionInfo == null) throw new ServerException($"not exist session. pid: {pid}");

                    // 세션 토큰 검사.
                    if (false == sessionInfo.AccessToken.Equals(GamePacket.Token)) throw new ServerException($"not exist session. pid: {pid}");

                    // UnitId 생성, 등록.
                    if (false == _server.RegisterPid(sessionId, sessionUid, pid)) throw new ServerException($"already exist player. pid: {pid}");

                    // Send Enter packet.
                    var enterPacket = _dataSerializer.Serialize(new GamePacketSC_Enter
                    {
                    });

                    _server.Send(sessionId, sessionUid, GamePacketCode.SC_Enter, enterPacket);
                }
                catch (ServerException ex)
                {
                    _logger.LogWarning(ex.Message);
                    _server.Disconnect(sessionId, sessionUid);

                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });
        }
    }
}
