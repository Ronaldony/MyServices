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
        /// 플레이어 대시 처리.
        /// </summary>
        private void Dash(PlayerUnit pu, GamePacketCS_Dash packet)
        {
            // Dash.
            if (false == pu.MovementHandler.Dash(packet.Position))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(pu.Section, GamePacketCode.SC_MoveFixed, new GamePacketSC_MoveFixed
                {
                    UnitId = pu.UnitId,
                    FixedPos = pu.MovementHandler.NowPos()
                });

                _logger.LogError($"dash - recv: {packet.Position}");
                return;
            }

            // 주변 섹션에 대시 전송.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_Dash, new GamePacketSC_Dash
            {
                UnitId = pu.UnitId,
                Direction = pu.MovementHandler.NowDir()
            });
        }
    }
}
