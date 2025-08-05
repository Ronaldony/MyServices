using System;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketAttribute.
    /// </summary>
    public class GamePacketAttribute : Attribute
    {
        public int PacketCode;

        public GamePacketAttribute(int packetCode) 
        {
            PacketCode = packetCode;
        }
    }
}
