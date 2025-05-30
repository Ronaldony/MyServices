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

            var position = worldPlayer.PlayInfo.Position;

            // 대시 결과 유효성 검사.
            if (false == IsValidDash(position.MoveDirection, worldPlayer.PlayInfo.Stat.Velocity, position.Pos, packet.Position))
            {
                _logger.LogError($"Dash - recv: {packet.Position}");
                return;
            }

            // 패킷.
            var section = ((int)position.Pos.X / GameData.SECTION_WIDTH, (int)position.Pos.Y / GameData.SECTION_HEIGHT);
            var packetBytes = _dataSerializer.Serialize(new NetPacketSC_Dash
            {
                Pid = packet.Pid,
                DashDirection = position.MoveDirection
            });

            // 주변 섹션에 넉백 전송.
            SendPacket_AroundSections(worldPlayer.GameSession, section, NetPacketCode.SC_Dash, packetBytes);

            position.Pos = packet.Position;
            position.MoveDirection = Type_MoveDirection.None;
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidDash(Type_MoveDirection moveDir, int velocity, Vector2 playerPos, Vector2 dashedPos)
        {
            // 대시 방향 계산 시퀀스.
            // 1. 대시 방향 확인.
            // 2. 대시 방향에 따라 오차 범위 값 계산.
            // 3. x, y 좌표에 대시 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.

            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // Y 좌표 검사.
                        if (dashedPos.Y > playerPos.Y + caclDist) { return false; }

                        // X 좌표 검사.
                        if (dashedPos.X > playerPos.X + GameData.MOVEMENT_ERROR) { return false; }
                        if (dashedPos.X + GameData.MOVEMENT_ERROR < playerPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // Y 좌표 검사.
                        if (dashedPos.Y + caclDist < playerPos.Y) { return false; }

                        // X 좌표 검사.
                        if (dashedPos.X > playerPos.X + GameData.MOVEMENT_ERROR) { return false; }
                        if (dashedPos.X + GameData.MOVEMENT_ERROR < playerPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X > playerPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y > playerPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                        if (dashedPos.Y + GameData.MOVEMENT_ERROR < playerPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X + caclDist < playerPos.X) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y > playerPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                        if (dashedPos.Y + GameData.MOVEMENT_ERROR < playerPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X > playerPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y > playerPos.Y + caclDist) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X + caclDist < playerPos.X) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y > playerPos.Y + caclDist) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X > playerPos.X + caclDist) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y + caclDist < playerPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var caclDist = GameData.DASH_TIME * GameData.DASH_SPEED * 0.8 * velocity + GameData.DASH_ERROR;

                        // X 좌표 검사.
                        if (dashedPos.X + caclDist < playerPos.X) { return false; }

                        // Y 좌표 검사.
                        if (dashedPos.Y + caclDist < playerPos.Y) { return false; }
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
