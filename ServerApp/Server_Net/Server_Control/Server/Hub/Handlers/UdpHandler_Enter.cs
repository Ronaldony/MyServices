using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Control.Server.Hub.Handlers
{
    using Framework.Core.Database.DAO;
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

        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDataSerializer _dataSerializer;

        private readonly DBSessionInfo _dbSessionInfo;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_Enter(IServiceProvider serviceProvider)
            : base(HubPacketCode.CS_Enter)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Enter>>();

            _server = serviceProvider.GetRequiredService<HubServer>();

            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _dbSessionInfo = serviceProvider.GetRequiredService<DBSessionInfo>();

            // Hub providers.
            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var hubPacket = _dataSerializer.Deserialize<HubPacketCS_Enter>(packet.Data);

            Task.Run(async () => 
            {
                _logger.LogDebug($"Player enter into hub");

                var pid = hubPacket.Pid;

                try
                {
                    // 세션.
                    var sessionInfo = await _dbSessionInfo.SelectAsync(pid);
                    if (sessionInfo == null) throw new ServerException($"not exist session. pid: {pid}");

                    // 세션 토큰 검사.
                    if (false == sessionInfo.AccessToken.Equals(hubPacket.Token)) throw new ServerException($"token not match. pid: {pid}, packet token: {hubPacket.Token}, json: {_jsonSerializer.Serialize(sessionInfo)}");

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
                    _logger.LogWarning(ex.Msg);
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
