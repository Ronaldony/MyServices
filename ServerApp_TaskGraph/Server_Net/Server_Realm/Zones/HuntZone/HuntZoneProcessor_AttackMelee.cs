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
            var aroundSections = GetAroundSections(pu.Section);
            var enemyUnits = new List<Unit>();

            foreach (var section in aroundSections)
            {
                enemyUnits.AddRange(_worldGroup.WorldSectionDict[pu.Section].GetMobUnits());
            }

            // 근정 공격.
            pu.AttackMelee(packet.Angle, enemyUnits);

            //공격 패킷 전송.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_AttackMelee,
                new GamePacketSC_AttackMelee
                {
                    Pid = pu.Pid,
                    Angle = packet.Angle
                });
        }
    }
}
