using Microsoft.Extensions.Logging;
using System.Numerics;

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
        /// 플레이어 넉백 처리.
        /// </summary>
        private void KnockBack(NetPacketCS_KnockBack packet)
        {
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}");
                return;
            }

            var playerPos = worldPlayer.PlayInfo.Position;

            // 운동량(P) = 질량(MASS) * 속도(V)
            // 위 공식은 Unity AddForce(Impulse)와 동일한 결과를 내기 위한 공식.
            var momentum = Vector2.Normalize(packet.Position - playerPos.Pos) * GameData.KNOCKBACK_THRUST * GameData.KNOCKBACK_MASS;

            // 넉백 예상 위치 = 현재 위치 + (운동량 * 움직인 시간)
            var calcPosition = playerPos.Pos + (momentum * GameData.KNOCKBACK_TIME);

            // 계산된 위치와 전달 받은 위치의 거리가 오차 허용 범위 내인지 검사.
            var errorPos = Vector2.Distance(calcPosition, packet.Position);

            // 오차 허용 범위 검사.
            if (GameData.KNOCKBACK_ERROR_DISTANCE < errorPos)
            {
                // Move fixed.
                SendPacket(packet.Pid, NetPacketCode.SC_MoveFixed, _dataSerializer.Serialize(new NetPacketSC_MoveFixed
                {
                    Pid = packet.Pid,
                    FixedPos = playerPos.Pos
                }));

                _logger.LogError($"KnockBack - calc: {calcPosition}, recv: {packet.Position}, momentum: {momentum} error: {errorPos}");
                return;
            }

            // 위치 갱신.
            playerPos.Pos = packet.Position;
            playerPos.MoveDirection = Type_MoveDirection.None;
        }
    }
}
