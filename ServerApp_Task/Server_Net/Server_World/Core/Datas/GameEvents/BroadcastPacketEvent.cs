
namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// BroadcastPacketEvent.
    /// 이벤트 처리.
    /// 1) 패킷 발생 섹션의 주변 섹션으로 패킷 전파.
    /// </summary>
    public class BroadcastPacketEvent : GameEvent
    {
        public BroadcastPacketEvent(SectionId section, int packetCode, byte[] packetBytes)
            : base (Type_GameEvent.BroadcastPacket)
        {
            RouteSection = section;
            PacketCode = packetCode;
            PacketBytes = packetBytes;
        }

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
