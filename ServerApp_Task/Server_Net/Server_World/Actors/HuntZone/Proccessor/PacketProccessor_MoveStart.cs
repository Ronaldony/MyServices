using GameCore;
using GameCore.NetPacket;
using Server_World.Core.Datas.GameEvents;
using Server_World.Core.Worlds;

namespace Server_World.Actors.HuntZone.Proccessor
{
    /// <summary>
    /// PacketProccessor.
    /// </summary>
    public partial class PacketProccessor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private MoveStartGameEvent MoveStart(World_Player worldPlayer, NetPacketCS_MoveStart packet)
        {
            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            //if (TimeUtil.IsPastTime(packet.PacketTime, playInfo.UpdateTime))
            //{
            //    _logger.LogError($"Packe time error. packet: {packet.PacketTime.ToString("hh:mm:ss.fff")}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}");
            //    return null;
            //}

            // PlayInfo 갱신.
            playInfo.UpdateTime = packet.PacketTime;
            location.MoveDirection = packet.MoveDirection;
            playInfo.Action = Type_Action.Move;

            return new MoveStartGameEvent
            {
                PacketTime = packet.PacketTime,
                MoveDirection = packet.MoveDirection,
                Action = Type_Action.Move,
            };

            // 근처 섹션의 플레이어.
            //var section = GetSection(location.Position);
            //var packetBytes = _dataSerializer.Serialize(
            //    new NetPacketSC_MoveStart
            //    {
            //        Pid = playInfo.Pid
            //    });

            // 패킷 전송.
            //SendPacket_AroundSections(worldPlayer.GameSession, section, NetPacketCode.SC_MoveStart, packetBytes);

            // Log.
            //_logger.LogDebug($"MoveStart - pid: {playInfo.Pid}, pos: {location.Position}, move: {packet.MoveDirection}, time: {packet.PacketTime.ToString("hh:mm:ss.fff")}");
        }
    }
}
