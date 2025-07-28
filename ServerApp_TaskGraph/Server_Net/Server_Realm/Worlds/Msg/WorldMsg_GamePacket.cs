
namespace Server_Realm.Worlds.Msg
{
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;

    public class WorldMsg_GamePacket : WorldMsgBase
    {
        public WorldMsg_GamePacket() 
            : base(Type_WorldMsg.GamePacket)
        {

        }
        /// <summary>
        /// UnitId.
        /// </summary>
        public long UnitId;

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode;  

        /// <summary>
        /// Packet.
        /// </summary>
        public GamePacketBase_CS Packet;
    }
}
