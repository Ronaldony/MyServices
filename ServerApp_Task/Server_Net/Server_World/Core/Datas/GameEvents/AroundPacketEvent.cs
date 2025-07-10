
namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// AroundPacketEvent.
    /// </summary>
    public class AroundPacketEvent : GameEvent
    {
        public AroundPacketEvent(SectionId section, int packetCode, byte[] packetBytes)
            : base (Type_GameEvent.RouteAroundPacket)
        {
            Section = section;
            PacketCode = packetCode;
            PacketBytes = packetBytes;
        }

        /// <summary>
        /// Section.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Pakcet bytes.
        /// </summary>
        public byte[] PacketBytes { get; set; }
    }
}
