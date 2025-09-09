
using Externals.UDP;

/// <summary>
/// GamePacketHandler.
/// </summary>
namespace GamePackets
{
    public abstract class GamePacketHandler : UdpHandler
    {
        public GamePacketHandler(int packetCode)
            : base(packetCode)
        {

        }
    }
}
