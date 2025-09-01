using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone
{
    /// <summary>
    /// HuntZoneHandler.
    /// </summary>
    public abstract class HuntZoneHandler : UdpHandler
    {
        public HuntZoneHandler(int packetCode)
            : base(packetCode)
        {
        }
    }
}
