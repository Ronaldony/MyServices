
namespace ServerEngine.Test.Network
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Test.
    /// </summary>
    public class UdpHandler_Test : UdpHandler
    {
        private readonly ILogger<UdpHandler_Test> _logger;
		private readonly IDataSerializer _dataSerializer;

        public UdpHandler_Test(IServiceProvider serviceProvider)
            : base(UdpPacketCode.Test)
        {
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Test>>();
        }

        /// <summary>s
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet, IUdpSession session, Func<int, ulong, UdpPacket, bool> sendFunc)
        {
            //Console.WriteLine($"Packet - code: {packet.PacketCode} / session: {session.SessionId}");

            var gameSession = session as GameSession;

            // Send to session.
            sendFunc(session.SessionId, session.SessionUid, packet);
        }
    }
}
