
namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
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

            // Process.
            if (Type_GameEvent.ReceivePacket == gameEvent.GameEventType)
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
                    case NetPacketCode.CS_KnockBack:
                        KnockBack(worldPlayer, packetEvent.Packet as NetPacketCS_KnockBack);
                        break;
                    case NetPacketCode.CS_Dash:
                        Dash(worldPlayer, packetEvent.Packet as NetPacketCS_Dash);
                        break;
                }
            }
            else
            {
                // Other event.
                switch (gameEvent.GameEventType)
                {
                    case Type_GameEvent.RouteAroundPacket:
                        SendPacket_AroundFromEvent(gameEvent as AroundPacketEvent);
                        break;
                    case Type_GameEvent.Attack:
                        break;
                    case Type_GameEvent.MoveSection:
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
