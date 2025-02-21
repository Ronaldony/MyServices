
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
        private readonly IDataSerializer _dataSerializer;

        public UdpHandler_Test(IServiceProvider serviceProvider)
            : base(UdpPacketCode.Test)
        {
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
        }

        /// <summary>s
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet, IUdpSession session, Func<int, UdpPacket, bool> sendFunc)
        {
            Console.WriteLine($"Packet - code: {packet.PacketCode} / session: {session.SessionId}");

            // Send to session.
            sendFunc(session.SessionId, packet);
        }
    }
}
