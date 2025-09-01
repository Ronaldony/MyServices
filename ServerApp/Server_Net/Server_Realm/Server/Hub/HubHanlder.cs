using ServerEngine.Network.UDP;

namespace Server_Realm.Server.Hub
{
    /// <summary>
    /// HubHanlder.
    /// </summary>
    public abstract class HubHanlder : UdpHandler
    {
        public HubHanlder(int packetCode)
            : base(packetCode)
        {
        }
    }
}
