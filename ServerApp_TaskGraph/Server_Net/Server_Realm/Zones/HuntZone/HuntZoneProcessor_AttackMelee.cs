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
        /// 플레이어 근접공격 처리.
        /// </summary>
        private void AttackMelee(PlayerUnit pu, GamePacketCS_AttackMelee packet)
        {
            // 근정 공격.
            pu.AttackMelee(packet.Angle);

            // 패킷 전송 - 공격.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_AttackMelee, new GamePacketSC_AttackMelee
            {
                UnitId = pu.UnitId,
                Angle = packet.Angle
            });
        }
    }
}
