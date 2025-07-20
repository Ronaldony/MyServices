
namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.GamePacket;
    using Server_World.Core.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process AttackEvent.
        /// </summary>
        public void Proc_AttackEvent(AttackEvent ge)
        {
            // 공격 패킷 전송.
            SendPacket_ManagedSections(GamePacketCode.SC_Attack, ge.GamePacket);

            // 근처 섹션 enemy 충돌 검사.
            var attackAroundSections = _router.GetAroundSections(ge.RouteSection);
            var hitEnemyDict = new Dictionary<SectionId, List<DTO_Enemy>>();

            // 충돌 박스.
            var collision = _collisionService.Rent(ge.WeaponCollisionType);
            collision.UpdateHitbox(ge.AttackerPos, ge.Angle);

            var hitbox = collision.GetHitbox();

            foreach (var hitSection in _worldGroup.ManagedSections)
            {
                // Hit 유닛 검사.
                if (attackAroundSections.Contains(hitSection))
                {
                    hitEnemyDict.Add(hitSection, new List<DTO_Enemy>());
                    HitEnemies(ge.Damage, hitSection, hitEnemyDict[hitSection], hitbox, ge.AttackTime);
                }
            }

            // 피격 패킷 전송.
            if (hitEnemyDict.Any())
            {
                var hitEnemyPacket = new GamePacketSC_HitEnemy();

                foreach (var pair in hitEnemyDict)
                {
                    // 피격 유닛이 있는 경우에만 전송.
                    if (pair.Value.Any())
                    {
                        hitEnemyPacket.Damage = ge.Damage;
                        hitEnemyPacket.Enemies = pair.Value;

                        SendPacket_AroundSections(pair.Key, GamePacketCode.SC_HitEnemy, hitEnemyPacket);
                    }
                }
            }

            // Release.
            _collisionService.Release(collision);
        }
    }
}
