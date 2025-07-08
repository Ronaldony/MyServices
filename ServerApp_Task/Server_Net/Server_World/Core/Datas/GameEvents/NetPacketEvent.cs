namespace Server_World.Core.Datas.GameEvents
{
    using GameCore.NetPacket;
    using Server_World.Core.Types;

    /// <summary>
    /// NetPacketEvent.
    /// </summary>
    public class NetPacketEvent : GameEvent
    {
        public NetPacketEvent()
            : base(Type_GameEvent.Packet)
        {

        }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Packet.
        /// </summary>
        public NetPacketBase_CS Packet { get; set; }
    }
}
