using Microsoft.Extensions.Logging;
using System.Numerics;

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
        /// 플레이어 투사체 공격 처리.
        /// </summary>
        private void AttackProjctile(PlayerUnit pu, GamePacketCS_AttackProjectile packet)
        {
            // 투사체 스폰 위치 검사.
            var distance = Vector2.DistanceSquared(pu.MovementHandler.NowPos(), packet.Projectile.SpawnPos);
            if (distance > GameData.PROJECTILE_SPAWN_DISTANCE_SQUARE)
            {
                // attack fail.
                _logger.LogError($"invalid projectile spawn position.");
                return;
            }

            // Create projectile.
            pu.CreateProjectile(packet.Projectile.SpawnPos, packet.Projectile.TargetPos);

            // 투사체 공격 패킷.
            var gamePacket = new GamePacketSC_AttackProjectile
            {
                Projectile = packet.Projectile,
            };

            // Send packet.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_AttackProjectile, gamePacket);
        }
    }
}
