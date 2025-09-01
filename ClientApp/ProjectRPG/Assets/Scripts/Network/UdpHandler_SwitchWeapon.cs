using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_SwitchWeapon.
    /// </summary>
    public class UdpHandler_SwitchWeapon : GamePacketHandler
    {
        public UdpHandler_SwitchWeapon()
            : base(GamePacketCode.SC_SwitchWeapon)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_SwitchWeapon>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_SwitchWeapon,
                Data = packetData,
            };


        }
    }
}
