using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// HuntZoneProcessor.
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
                pu.WorldSection.SendPacket_AroundSections(GamePacketCode.SC_MoveFixed, new GamePacketSC_MoveFixed
                {
                    UnitId = pu.UnitId,
                    FixedPos = pu.MovementHandler.NowPos()
                });

                _logger.LogError($"dash - recv: {packet.Position}");
                return;
            }

            _logger.LogDebug($"Projectile. endpos: {pu.MovementHandler.NowPos()}");

            // 패킷 전송 - 대시.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_Dash, new GamePacketSC_Dash
            {
                UnitId = pu.UnitId,
                DashEnd = pu.MovementHandler.NowPos()
            });
        }
    }
}
