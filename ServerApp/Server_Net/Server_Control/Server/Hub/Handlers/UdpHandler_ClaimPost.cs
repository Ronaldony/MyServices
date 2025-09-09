using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Control.Server.Hub.Handlers
{
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using GameCore.HubPacket;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// UdpHandler_ClaimPost.
    /// </summary>
    public class UdpHandler_ClaimPost : HubHanlder
    {
        private readonly ILogger<UdpHandler_Enter> _logger;

        private readonly HubServer _server;

        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_ClaimPost(IServiceProvider serviceProvider)
            : base(HubPacketCode.CS_ClaimPost)
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
            var hubPacket = _dataSerializer.Deserialize<HubPacketCS_ClaimPost>(packet.Data);

            // Tell message.
            _playHubProvider.Tell<HubMsg_ClaimPost>(new HubMsg_ClaimPost
            {
                PostUid = hubPacket.PostUid,
                TargetPid = hubPacket.Pid
            });
        }
    }
}
