
namespace Server_World.Actors.HuntZone.Game
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds.Msg;

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
            // World player.
            var worldPlayer = _worldGroup.PlayerDict.GetValueOrDefault(gameEvent.Pid);
            if (default == worldPlayer)
            {
                return;
            }

            if (Type_GameEvent.Packet == gameEvent.GameEventType)
            {
                // PacketEvent.
                var packetEvent = gameEvent as NetPacketEvent;
                switch (packetEvent.PacketCode)
                {
                    case NetPacketCode.CS_MoveStart:
                        MoveStart(worldPlayer, packetEvent.Packet as NetPacketCS_MoveStart);
                        break;
                    case NetPacketCode.CS_MoveEnd:
                        MoveEnd(worldPlayer, packetEvent.Packet as NetPacketCS_MoveEnd);
                        break;
                    case NetPacketCode.CS_Attack:
                        Attack(worldPlayer, packetEvent.Packet as NetPacketCS_Attack);
                        break;
                    //case NetPacketCode.CS_KnockBack:
                    //    KnockBack(worldPlayer, packetEvent.Packet as NetPacketCS_KnockBack);
                    //    break;
                    //case NetPacketCode.CS_Dash:
                    //    Dash(worldPlayer, packetEvent.Packet as NetPacketCS_Dash);
                    //    break;
                }
            }
            else
            {
                // Other event.
                switch (gameEvent.GameEventType)
                {
                    case Type_GameEvent.AroundPacket:
                        SendPacket_AroundFromEvent(gameEvent as AroundPacketEvent);
                        break;
                }
            }

        }

        /// <summary>
        /// Process WorldMsg.
        /// </summary>
        public void ProcessWorldMsg(WorldMsg worldMsg)
        {
            switch (worldMsg.MsgType)
            { 

            }
        }
    }
}
