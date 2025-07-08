
namespace Server_World.Actors.HuntZone.Processor
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// GameProcessor.
    /// </summary>
    public partial class GameProcessor
    {
        /// <summary>
        /// Process GameEvent.
        /// </summary>
        public void ProcessGameEvent(GameEvent gameEvent)
        {
            switch (msg.PacketCode)
            {
                case NetPacketCode.CS_MoveStart:
                    MoveStart(worldPlayer, msg.Packet as NetPacketCS_MoveStart);
                    break;
                case NetPacketCode.CS_MoveEnd:
                    MoveEnd(worldPlayer, msg.Packet as NetPacketCS_MoveEnd);
                    break;
                case NetPacketCode.CS_Attack:
                    Attack(worldPlayer, msg.Packet as NetPacketCS_Attack);
                    break;
                case NetPacketCode.CS_KnockBack:
                    KnockBack(worldPlayer, msg.Packet as NetPacketCS_KnockBack);
                    break;
                case NetPacketCode.CS_Dash:
                    Dash(worldPlayer, msg.Packet as NetPacketCS_Dash);
                    break;
            }
        }
    }
}
