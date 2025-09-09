using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Control.Server.Hub.Handlers
{
    using Framework.Core.Database.DataObjects;
    using Framework.Core.Exceptions;
    using GameCore;
    using GameCore.HubPacket;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// UdpHandler_Enter.
    /// </summary>
    public class UdpHandler_Enter : HubHanlder
    {
        private readonly ILogger<UdpHandler_Enter> _logger;

        private readonly HubServer _server;

        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_Enter(IServiceProvider serviceProvider)
            : base(HubPacketCode.CS_Enter)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Enter>>();

            _server = serviceProvider.GetRequiredService<HubServer>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Session object.
            _sessionInfoObject = serviceProvider.GetRequiredService<SessionInfoObject>();

            // Hub providers.
            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            Task.Run(async () => 
            {
                _logger.LogDebug($"Player enter into hub");

                var hubPacket = _dataSerializer.Deserialize<HubPacketCS_Enter>(packet.Data);
                var pid = hubPacket.Pid;

                try
                {
                    // 세션.
                    await _sessionInfoObject.LoadDataAsync(pid);

                    var sessionInfo = _sessionInfoObject.SessionInfo;
                    if (sessionInfo == null) throw new ServerException($"not exist session. pid: {pid}");

                    // 세션 토큰 검사.
                    if (false == sessionInfo.AccessToken.Equals(hubPacket.Token)) throw new ServerException($"not exist session. pid: {pid}");

                    // UnitId 생성, 등록.
                    if (false == _server.RegisterPid(sessionId, sessionUid, pid)) throw new ServerException($"already exist player. pid: {pid}");

                    // HubMsg.
                    var enterMsg = new HubMsg_Enter
                    {
                        TargetPid = pid,
                    };

                    // Hub 메시지 전송.
                    _playHubProvider.Tell<HubMsg_Enter>(enterMsg);
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
