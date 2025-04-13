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
        /// 플레이어 대시 처리.
        /// </summary>
        private void Dash(NetPacketCS_Dash packet)
        {
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}");
                return;
            }

            var playInfo = worldPlayer.PlayInfo;
            var position = worldPlayer.PlayInfo.Position;

            // 대시 결과 유효성 검사.
            if (false == IsValidDash(position.MoveDirection, playInfo.Stat.Velocity, position.Pos, packet.Position))
            {
                _logger.LogError($"Dash - recv: {packet.Position}");
                return;
            }

            //position.Pos = packet.Position;

            // 주변 섹션에 대시 전송.
            //SendPacket(worldPlayer.GameSession.SessionId, worldPlayer.GameSession.SessionUid, NetPacketCode.SC_KnockBack, _dataSerializer.Serialize(sendPacket));
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidDash(Type_MoveDirection moveDir, int velocity, Vector2 playerPos, Vector2 dashedPos)
        {
            // 이동 방향 계산 시퀀스.
            // 1. 대시 방향 확인.
            // 2. 대시 방향에 따라 오차 범위 값 계산.
            // 3. x, y 좌표에 대시 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.

            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // Y 좌표 검사.
                        if (playerPos.Y > dashedPos.Y + caclDist) { return false; }

                        // X 좌표 검사.
                        if (playerPos.X > dashedPos.X + GameData.MOVENT_ERROR) { return false; }
                        if (playerPos.X + GameData.MOVENT_ERROR < dashedPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // Y 좌표 검사.
                        if (playerPos.Y + caclDist < dashedPos.Y) { return false; }

                        // X 좌표 검사.
                        if (playerPos.X > dashedPos.X + GameData.MOVENT_ERROR) { return false; }
                        if (playerPos.X + GameData.MOVENT_ERROR < dashedPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X > dashedPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y > dashedPos.Y + GameData.MOVENT_ERROR) { return false; }
                        if (playerPos.Y + GameData.MOVENT_ERROR < dashedPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X + caclDist < dashedPos.X) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y > dashedPos.Y + GameData.MOVENT_ERROR) { return false; }
                        if (playerPos.Y + GameData.MOVENT_ERROR < dashedPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X > dashedPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y > dashedPos.Y + caclDist) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X + caclDist < dashedPos.X) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y > dashedPos.Y + caclDist) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X > dashedPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y + caclDist < dashedPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (playerPos.X + caclDist < dashedPos.X) { return false; }

                        // Y 좌표 검사.
                        if (playerPos.Y + caclDist < dashedPos.Y) { return false; }
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
