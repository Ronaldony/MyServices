
namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
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
            // Process.
            if (Type_GameEvent.ReceivePacket == gameEvent.GetGameEventType())
            {
                // World player.
                var worldPlayer = _worldGroup.PlayerDict.GetValueOrDefault(gameEvent.Pid);
                if (default == worldPlayer)
                {
                    return;
                }

                // Receive PacketEvent.
                var packetEvent = gameEvent as ReceivePacketEvent;

                switch (packetEvent.PacketCode)
                {
                    case GamePacketCode.CS_MoveStart:
                        MoveStart(worldPlayer, packetEvent.Packet as GamePacketCS_MoveStart);
                        break;
                    case GamePacketCode.CS_MoveEnd:
                        MoveEnd(worldPlayer, packetEvent.Packet as GamePacketCS_MoveEnd);
                        break;
                    case GamePacketCode.CS_Attack:
                        Attack(worldPlayer, packetEvent.Packet as GamePacketCS_Attack);
                        break;
                    case GamePacketCode.CS_KnockBack:
                        KnockBack(worldPlayer, packetEvent.Packet as GamePacketCS_KnockBack);
                        break;
                    case GamePacketCode.CS_Dash:
                        Dash(worldPlayer, packetEvent.Packet as GamePacketCS_Dash);
                        break;
                }
            }
            else
            {
                // Other event.
                switch (gameEvent.GetGameEventType())
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
