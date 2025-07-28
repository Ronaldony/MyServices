
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
        /// 플레이어 넉백 처리.
        /// </summary>
        private void KnockBack(PlayerUnit pu, GamePacketCS_KnockBack packet)
        {
            // 섹션 검증.
            if (false == IsValidSection(packet.Position))
            {
                _logger.LogError($"invalid section. pos: {packet.Position}");
                return;
            }

            // Knockback.
            if (false == pu.MovementHandler.Knockback(packet.Position))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(pu.Section, GamePacketCode.SC_MoveFixed, new GamePacketSC_MoveFixed
                {
                    UnitId = pu.UnitId,
                    FixedPos = pu.MovementHandler.NowPos()
                });

                return;
            }
        }
    }
}
