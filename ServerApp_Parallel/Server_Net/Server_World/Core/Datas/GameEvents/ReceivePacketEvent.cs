namespace Server_World.Core.Datas.GameEvents
{
    using GameCore.GamePacket;
    using Server_World.Core.Types;

    /// <summary>
    /// ReceivePacketEvent.
    /// </summary>
    public class ReceivePacketEvent : GameEvent
    {
        public ReceivePacketEvent()
            : base(Type_GameEvent.ReceivePacket)
        {

        }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Packet.
        /// </summary>
        public GamePacketBase_CS Packet { get; set; }
    }
}
