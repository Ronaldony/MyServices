
using Externals.UDP;

/// <summary>
/// HubPacketHandler.
/// </summary>
public abstract class HubPacketHandler : UdpHandler
{
    public HubPacketHandler(int packetCode)
        : base(packetCode)
    {

    }
}