using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// 플레이어 넉백 처리.
        /// </summary>
        private void KnockBack(World_Player worldPlayer, NetPacketCS_KnockBack packet)
        {
            var playInfo = worldPlayer.PlayInfo;
            var position = worldPlayer.PlayInfo.Position;

            // 운동량(P) = 질량(MASS) * 속도(V)
            // 위 공식은 Unity AddForce(Impulse)와 동일한 결과를 내기 위한 공식.
            var momentum = Vector2.Normalize(packet.Position - position) * GameData.KNOCKBACK_THRUST * GameData.KNOCKBACK_MASS;

            // 넉백 예상 위치 = 현재 위치 + (운동량 * 움직인 시간)
            var calcPosition = position + (momentum * GameData.KNOCKBACK_TIME);

            // 계산된 위치와 전달 받은 위치의 거리가 오차 허용 범위 내인지 검사.
            var errorPos = Vector2.Distance(calcPosition, packet.Position);

            // 오차 허용 범위 검사.
            if (GameData.KNOCKBACK_ERROR_DISTANCE < errorPos)
            {
                // Move fixed.
                SendPacket(playInfo.Pid, NetPacketCode.SC_MoveFixed, _dataSerializer.Serialize(new NetPacketSC_MoveFixed
                {
                    Pid = playInfo.Pid,
                    FixedPos = position
                }));

                _logger.LogError($"KnockBack - calc: {calcPosition}, recv: {packet.Position}, momentum: {momentum} error: {errorPos}");
                return;
            }

            // 위치 갱신.
            playInfo.Position = packet.Position;
            playInfo.MoveDirection = Type_MoveDirection.None;
        }
    }
}
