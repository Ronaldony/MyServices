using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
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
                    Pid = pu.Pid,
                    FixedPos = pu.MovementHandler.NowPos()
                });

                _logger.LogError($"dash - recv: {packet.Position}");
                return;
            }

            // 주변 섹션에 대시 전송.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_Dash, new GamePacketSC_Dash
            {
                Pid = pu.Pid,
                Direction = pu.MovementHandler.NowDir()
            });
        }
    }
}
