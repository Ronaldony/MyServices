using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 플레이어 투사체 공격 처리.
        /// </summary>
        private void AttackProjctile(World_Player worldPlayer, GamePacketCS_AttackProjectile packet)
        {
            var nowTime = TimeUtil.Now;

            // 플레이어.
            var playInfo = worldPlayer.PlayInfo;
            var pos = playInfo.Location.Position;

            // 투사체 스폰 위치 검사.
            var distance = (pos - packet.SpawnPos).LengthSquared();
            if (distance > GameData.PROJECTILE_SPAWN_DISTANCE_SQUARE)
            {
                // attack fail.
                _logger.LogError($"invalid projectile spawn position.");
                return;
            }

            // 투사체 생성.
            var tbWeapon = _tdbService.GetById<TB_Weapon>(playInfo.Equipment.Weapon);
            var projectile = _projectileService.Rent(tbWeapon.ProjectileType, worldPlayer.Pid, packet.SpawnPos, packet.Angle, tbWeapon.Damage);

            if (null == projectile)
            {
                _logger.LogError($"invalid weapon. weapon: {tbWeapon.Name}");
                return;
            }

            // 스폰 투사체 추가.
            _worldProjectile.SpawnProjectiles.Add(projectile);

            // 공격 패킷 전송.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, GamePacketCode.SC_AttackProjectile,
                 new GamePacketSC_AttackProjectile
                 {
                     Pid = worldPlayer.Pid,
                     ProjectileId = projectile.ProjectileId,
                     SpawnPos = packet.SpawnPos,
                     Angle = packet.Angle
                 });
        }
    }
}
