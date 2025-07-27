
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
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Packet.
        /// </summary>
        public GamePacketBase_CS Packet { get; set; }
    }
}
