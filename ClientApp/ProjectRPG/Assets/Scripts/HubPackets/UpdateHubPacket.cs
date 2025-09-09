
namespace HubPackets
{
    /// <summary>
    /// UpdateHubPacket.
    /// </summary>
    public class UpdateHubPacket
    {
        public int PacketCode { get; set; }

        public object Data { get; set; }
    }
}