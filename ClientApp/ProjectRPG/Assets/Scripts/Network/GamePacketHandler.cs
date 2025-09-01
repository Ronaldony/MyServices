
using Externals.UDP;

/// <summary>
/// GamePacketHandler.
/// </summary>
public abstract class GamePacketHandler : UdpHandler
{
    public GamePacketHandler(int packetCode)
        : base(packetCode)
    {

    }
}