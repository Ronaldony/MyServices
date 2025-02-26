namespace ServerEngine.Network.UDP
{
	/// <summary>
	/// UdpPacket.
	/// </summary>
	public class UdpPacket
    {
		/// <summary>
		/// Packet Code.
		/// </summary>
		public int PacketCode { get; set; }

		/// <summary>
		/// Packet data.
		/// </summary>
		public byte[] Data { get; set; }
    }
}
