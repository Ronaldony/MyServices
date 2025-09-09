
using Externals.UDP;

/// <summary>
/// HubPacketHandler.
/// </summary>
namespace HubPackets
{
    public abstract class HubPacketHandler : UdpHandler
    {
        public HubPacketHandler(int packetCode)
            : base(packetCode)
        {

        }
    }
}
