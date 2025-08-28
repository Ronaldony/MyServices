using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_PlayHub.Server.Hub.Handlers
{
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.HubPacket;
    using Server_PlayHub.Hubs;
    using Server_PlayHub.Hubs.Msg;

    /// <summary>
    /// UdpHandler_DeletePost.
    /// </summary>
    public class UdpHandler_DeletePost : UdpHandler
    {
        private readonly ILogger<UdpHandler_Enter> _logger;

        private readonly HubServer _server;

        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_DeletePost(IServiceProvider serviceProvider)
            : base(HubPacketCode.CS_DeletePost)
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
            var hubPacket = _dataSerializer.Deserialize<HubPacketCS_DeletePost>(packet.Data);

            // Tell message.
            _playHubProvider.Tell<HubMsg_DeletePost>(new HubMsg_DeletePost
            {
                PostUid = hubPacket.PostUid,
                TargetPid = hubPacket.Pid
            });
        }
    }
}
