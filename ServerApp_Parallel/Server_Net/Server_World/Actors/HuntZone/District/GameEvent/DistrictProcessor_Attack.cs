
namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process AttackEvent.
        /// </summary>
        public void Proc_AttackEvent(AttackEvent attackEvent)
        {
            // 공격 패킷 전송.
            SendPacket_ManagedSections(NetPacketCode.SC_Attack, attackEvent.NetPacket);

            // 근처 섹션 enemy 충돌 검사.
            var attackAroundSections = _router.GetAroundSections(attackEvent.RouteSection);
            var hitEnemyDict = new Dictionary<SectionId, List<DTO_Enemy>>();

            foreach (var hitSection in _worldGroup.ManagedSections)
            {
                // Hit 유닛 검사.
                if (attackAroundSections.Contains(hitSection))
                {
                    hitEnemyDict.Add(hitSection, new List<DTO_Enemy>());
                    HitEnemies(attackEvent.Damage, hitSection, hitEnemyDict[hitSection], attackEvent.BoudingBox, attackEvent.AttackTime);
                }
            }

            // 피격 패킷 전송.
            if (hitEnemyDict.Any())
            {
                var hitEnemyPacket = new NetPacketSC_HitEnemy();

                foreach (var pair in hitEnemyDict)
                {
                    // 피격 유닛이 있는 경우에만 전송.
                    if (pair.Value.Any())
                    {
                        hitEnemyPacket.Damage = attackEvent.Damage;
                        hitEnemyPacket.Enemies = pair.Value;

                        SendPacket_AroundSections(pair.Key, NetPacketCode.SC_HitEnemy, hitEnemyPacket);
                    }
                }
            }
        }
    }
}
