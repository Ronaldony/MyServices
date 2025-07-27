
using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveStart(PlayerUnit pu, GamePacketCS_MoveStart packet)
        {
            // Start move.
            pu.MovementHandler.MoveStart(packet.Direction);

            // log.
            _logger.LogDebug($"MoveStart - pos: {pu.MovementHandler.NowPos()}, move: {packet.Direction}");

            // 패킷 전송 - 이동 시작.
            SendPacket_AroundSections(pu.Pid, pu.MovementHandler.Section(), GamePacketCode.SC_MoveStart, new GamePacketSC_MoveStart
            {
                Pid = pu.Pid,
                Direction = packet.Direction
            });
        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveEnd(PlayerUnit pu, GamePacketCS_MoveEnd packet)
        {
            //_logger.LogDebug($"MoveEnd - pos: {packet.EndPos}");

            // 섹션 검증
            if (false == IsValidSection(packet.EndPos))
            {
                _logger.LogError($"Invalid section. section: {packet.EndPos}");
                return;
            }

            // 이동 정지.
            if (false == pu.MovementHandler.MoveStop(packet.EndPos))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(pu.Section, GamePacketCode.SC_MoveFixed, new GamePacketSC_MoveFixed
                {
                    Pid = pu.Pid,
                    FixedPos = pu.MovementHandler.NowPos()
                });

                _logger.LogError($"Move end position error. ");
                _logger.LogError($"ori: {pu.MovementHandler.NowPos()}, dst: {packet.EndPos}, dir: {pu.MovementHandler.NowDir()}");
                return;
            }

            // 패킷 전송 - 이동 정지.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_MoveEnd, new GamePacketSC_MoveEnd
            {
                Pid = pu.Pid,
                EndPos = packet.EndPos
            });
        }
    }
}
