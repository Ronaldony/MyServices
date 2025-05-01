
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void Attack(NetPacketCS_Attack packet)
        {
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}");
                return;
            }

            var playerInfo = worldPlayer.PlayInfo;
            var playerPos = worldPlayer.PlayInfo.Position.Pos;

            // 충돌 범위 계산.
            var collisionBox = _collisionService.CalcCollisionBox(playerInfo.Equipment.Weapon, playerPos, packet.Angle);

            // 피격 가능 플레이어 계산.
            var section = ((int)playerPos.X / GameData.SECTION_WIDTH, (int)playerPos.Y / GameData.SECTION_HEIGHT);
            var aroundSections = GetAroundSections(section);

            foreach (var hitSection in aroundSections)
            {

            }

            // 피격 계산.
        }

    }
}
