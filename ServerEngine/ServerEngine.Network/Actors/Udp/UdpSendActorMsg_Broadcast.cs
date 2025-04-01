namespace ServerEngine.Network.Actors.Udp
{
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpSendActorMsg_Broadcast.
    /// </summary>
    public class UdpSendActorMsg_Broadcast
    {
        /// <summary>
        /// Session.
        /// </summary>
        public UdpSession Session { get; set; }

        /// <summary>
        /// Session id.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Session uid.
        /// </summary>
        public ulong SessionUid { get; set; }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Packet data.
        /// </summary>
        public byte[] PacketData { get; set; }
    }
}
