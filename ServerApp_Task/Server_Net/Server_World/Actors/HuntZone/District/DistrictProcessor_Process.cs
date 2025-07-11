
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
                // Receive PacketEvent.
                var packetEvent = gameEvent as ReceivePacketEvent;

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
                    case Type_GameEvent.BroadcastPacket:
                        SendPacket_AroundFromEvent(gameEvent as BroadcastPacketEvent);
                        break;
                    case Type_GameEvent.MoveSection:
                        Proc_MoveSectionEvent(gameEvent as MoveSectionEvent);
                        break;
                    case Type_GameEvent.MoveDistrict:
                        Proc_MoveDistrictEvent(gameEvent as MoveDistrictEvent);
                        break;
                    case Type_GameEvent.Attack:
                        Proc_AttackEvent(gameEvent as AttackEvent);
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
                case Type_WorldMsg.SpawnPlayer:
                    SpawnPlayer(worldMsg as WorldMsg_SpawnPlayer);
                    break;
                case Type_WorldMsg.OutPlayer:
                    OutPlayer(worldMsg as WorldMsg_OutPlayer);
                    break;
                case Type_WorldMsg.SpawnEnemy:
                    SpawnEnemy(worldMsg as WorldMsg_SpawnEnemy);
                    break;
                case Type_WorldMsg.UpdateFrame:
                    UpdateFrame(worldMsg as WorldMsg_UpdateFrame);
                    break;
                case Type_WorldMsg.UpdateMovePattern:
                    UpdateMovePattern(worldMsg as WorldMsg_UpdateMovePattern);
                    break;
                case Type_WorldMsg.UpdateDummy:
                    UpdateDummy(worldMsg as WorldMsg_UpdateDummy);
                    break;
            }
        }
    }
}
