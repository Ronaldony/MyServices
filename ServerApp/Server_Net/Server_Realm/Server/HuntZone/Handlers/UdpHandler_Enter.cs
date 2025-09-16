using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Exceptions;
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Server.HuntZone;

    /// <summary>
    /// UdpHandler_Enter.
    /// </summary>
    public class UdpHandler_Enter : HuntZoneHandler
    {
        private readonly ILogger<UdpHandler_Enter> _logger;

        private readonly HuntZoneServer _huntZoneServer;

        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDataSerializer _dataSerializer;

        // DB Objects.
        private readonly DBSessionInfo _dbSessionInfo;
        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        public UdpHandler_Enter(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_Enter)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Enter>>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Data object 생성.
            _dbSessionInfo = serviceProvider.GetRequiredService<DBSessionInfo>();
            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var gp = _dataSerializer.Deserialize<GamePacketCS_Enter>(packet.Data);

            Task.Run(async () => 
            {
                _logger.LogDebug($"Player Enter");

                var pid = gp.Pid;

                try
                {
                    var sessionInfo = await _dbSessionInfo.SelectAsync(pid);
                    if (sessionInfo == null) throw new ServerException($"not exist session. pid: {pid}");

                    // 세션 토큰 검사.
                    if (false == sessionInfo.AccessToken.Equals(gp.Token)) throw new ServerException($"token not match. pid: {pid}, packet token: {gp.Token}, json: {_jsonSerializer.Serialize(sessionInfo)}");

                    // UnitId 생성, 등록.
                    if (false == _huntZoneServer.RegisterPid(sessionId, sessionUid, pid)) throw new ServerException($"already exist player. pid: {pid}");

                    ////////////////////////////////////////////////////////////////////
                    /// Load info datas.

                    // PlayInfo.
                    var playInfo = await _dbPlayInfo.SelectAsync(pid);
                    if (null == playInfo) throw new ServerException($"not exist PlayInfo. pid: {pid}");

                    // InventoryInfo.
                    var inventoryInfo = await _dbInventoryInfo.SelectAsync(pid);
                    if (null == inventoryInfo) throw new ServerException($"not exist InventoryInfo. pid: {pid}");

                    // Send Enter packet.
                    var enterPacket = _dataSerializer.Serialize(new GamePacketSC_Enter
                    {
                        PlayInfo = playInfo,
                        InventoryInfo = inventoryInfo
                    });

                    _huntZoneServer.Send(sessionId, sessionUid, GamePacketCode.SC_Enter, enterPacket);
                }
                catch (ServerException ex)
                {
                    _logger.LogWarning(ex.Msg);
                    _huntZoneServer.Disconnect(sessionId, sessionUid);

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
