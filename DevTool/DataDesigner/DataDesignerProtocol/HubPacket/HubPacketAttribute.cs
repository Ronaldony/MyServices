using System;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketAttribute.
    /// </summary>
    public class HubPacketAttribute : Attribute
    {
        public int PacketCode;

        public HubPacketAttribute(int packetCode)
        {
            PacketCode = packetCode;
        }
    }
}
