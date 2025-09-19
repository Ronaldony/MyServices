using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Control.Server.Hub.Handlers
{
    using GameCore;
    using GameCore.HubPacket;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// UdpHandler_EnterZone.
    /// </summary>
    public class UdpHandler_EnterZone : HubHanlder
    {
        private readonly IDataSerializer _dataSerializer;

        private readonly PlayHubProvider _playHubProvider;

        public UdpHandler_EnterZone(IServiceProvider serviceProvider)
           : base(HubPacketCode.CS_EnterZone)
        {
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // Hub providers.
            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var hubPacket = _dataSerializer.Deserialize<HubPacketCS_EnterZone>(packet.Data);

            // Tell message.
            _playHubProvider.Tell<HubMsg_EnterZone>(new HubMsg_EnterZone
            {
                ZoneId = hubPacket.ZoneId,
                TargetPid = hubPacket.Pid
            });
        }
    }
}
