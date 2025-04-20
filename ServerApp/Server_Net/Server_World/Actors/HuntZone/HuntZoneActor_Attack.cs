
namespace Server_World.Actors.HuntZone
{
    using GameCore.NetPacket;
    using Microsoft.Extensions.Logging;
    using Server_World.Core.Worlds;
    using System.Numerics;

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

            var position = worldPlayer.PlayInfo.Position;

            // TODO: 공격 범위 계산.
            // TODO: 피격 가능 플레이어 계산.
            // TODO: 피격 계산.
        }
    }
}
