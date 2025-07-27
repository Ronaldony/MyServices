using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Numerics;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Projectiles;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 플레이어 투사체 공격 처리.
        /// </summary>
        private void AttackProjctile(PlayerUnit pu, GamePacketCS_AttackProjectile packet)
        {
            var nowTime = TimeUtil.Now;

            // 플레이어.
            var playInfo = pu.PlayInfo;
            var pos = pu.Position;

            // 투사체 스폰 위치 검사.
            var distance = Vector2.DistanceSquared(pos, packet.SpawnPos);
            if (distance > GameData.PROJECTILE_SPAWN_DISTANCE_SQUARE)
            {
                // attack fail.
                _logger.LogError($"invalid projectile spawn position.");
                return;
            }

            // 투사체 생성.
            var tbWeapon = _tdbService.GetById<TB_Weapon>(playInfo.Equipment.Weapon);
            var projectile = _projectileService.Rent(tbWeapon.ProjectileType, pu.Pid, packet.SpawnPos, packet.Angle, tbWeapon.Damage);

            if (null == projectile)
            {
                _logger.LogError($"invalid weapon. weapon: {tbWeapon.Name}");
                return;
            }

            // 스폰 투사체 추가.
            _worldProjectile.SpawnProjectiles.Add(projectile);

            // 투사체 공격 패킷.
            var gamePacket = new GamePacketSC_AttackProjectile();

            // Set projectile.
            gamePacket.Projectile = _memoryPoolService.Rent<DTO_Projectile>();
            gamePacket.Projectile.ProjectileId = projectile.ProjectileId;
            gamePacket.Projectile.SpawnPos = packet.SpawnPos;
            gamePacket.Projectile.Angle = packet.Angle;

            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_AttackProjectile, gamePacket);

            // Release.
            _memoryPoolService.Release(gamePacket.Projectile);
        }
    }
}
