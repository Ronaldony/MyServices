namespace GamePackets
{
    /// <summary>
    /// UpdateGamePacket.
    /// </summary>
    public class UpdateGamePacket
    {
        public int PacketCode { get; set; }

        public object Data { get; set; }
    }
}