

namespace ServerEngine.Test.Network
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;
    using ServerEngine.Test.Database.Data;

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
        public override void HandlePacket<TSession>(UdpPacket packet, TSession session)
        {
            Console.WriteLine($"Packet - code: {packet.PacketCode}");

            var playerInfo = _dataSerializer.Deserialize<DTO_PlayerInfo>(packet.Data);
        }
    }
}
