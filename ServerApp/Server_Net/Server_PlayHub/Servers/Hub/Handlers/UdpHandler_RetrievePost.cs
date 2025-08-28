using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_PlayHub.Server.Hub.Handlers
{
    using Framework.Core.Database.DataObjects;
    using Framework.Core.Exceptions;
    using GameCore;
    using GameCore.HubPacket;
    using Server_PlayHub.Hubs;
    using Server_PlayHub.Hubs.Msg;

    /// <summary>
    /// UdpHandler_RetrievePost.
    /// </summary>
    public class UdpHandler_RetrievePost : UdpHandler
    {
        private readonly ILogger<UdpHandler_Enter> _logger;

        private readonly HubServer _server;

        private readonly IDataSerializer _dataSerializer;

        // Data objects.
        private readonly SessionInfoObject _sessionInfoObject;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_RetrievePost(IServiceProvider serviceProvider)
            : base(HubPacketCode.CS_RetrievePost)
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
            var hubPacket = _dataSerializer.Deserialize<HubPacketCS_RetrievePost>(packet.Data);

            // Tell message.
            _playHubProvider.Tell<HubMsg_RetrievePost>(new HubMsg_RetrievePost
            {
                Post = hubPacket.Post,
                TargetPid = hubPacket.Pid
            });
        }
    }
}
