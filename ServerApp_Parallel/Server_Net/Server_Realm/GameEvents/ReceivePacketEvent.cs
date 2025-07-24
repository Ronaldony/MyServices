namespace Server_Realm.GameEvents
{
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;

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
