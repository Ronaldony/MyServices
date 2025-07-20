using Microsoft.Extensions.Logging;
using System.Numerics;

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
        /// 플레이어 대시 처리.
        /// </summary>
        private void Dash(World_Player worldPlayer, GamePacketCS_Dash packet)
        {
            // 섹션 검증.
            if (false == IsValidSection(packet.Position))
            {
                _logger.LogError($"invalid dash. pos: {packet.Position}");
                return;
            }

            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;

            // 대시 결과 유효성 검사.
            if (false == IsValidDash(worldPlayer.PlayInfo.Stat.Velocity, location.MoveDirection, location.Position, packet.Position))
            {
                _logger.LogError($"dash - recv: {packet.Position}");
                return;
            }

            // 주변 섹션에 대시 전송.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, GamePacketCode.SC_Dash, new GamePacketSC_Dash
            {
                Pid = worldPlayer.Pid,
                Direction = location.MoveDirection
            });

            // 위치 갱신.
            location.PositionEnd = packet.Position;
            playInfo.Action = Type_Action.Dash;
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidDash(int velocity, Vector2 direction, Vector2 playerPos, Vector2 dashedPos)
        {
            var dirY = direction.Y;
            var dirX = direction.X;

            // Y 축 계산.
            if (dirY == 0)
            {
                // 고정축 범위 계산.
                if (dashedPos.Y > playerPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                if (dashedPos.Y + GameData.MOVEMENT_ERROR < playerPos.Y) { return false; }
            }
            else
            {
                var calcDist = dirY * (GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR);

                // Y 좌표 검사.
                if (dirY > 0)
                {
                    // Top.
                    if (dashedPos.Y > playerPos.Y + calcDist) { return false; }
                }
                else
                {
                    // Down.
                    if (dashedPos.Y + calcDist < playerPos.Y) { return false; }
                }
            }

            // X 축 계산.
            if (dirX == 0)
            {
                // 고정축 범위 계산.
                if (dashedPos.X > playerPos.X + GameData.MOVEMENT_ERROR) { return false; }
                if (dashedPos.X + GameData.MOVEMENT_ERROR < playerPos.X) { return false; }
            }
            else
            {
                var calcDist = dirX * (GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR);

                if (dirX > 0)
                {
                    // Right.
                    if (dashedPos.X > playerPos.X + calcDist) { return false; }
                }
                else
                {
                    // Left.
                    if (dashedPos.X + calcDist < playerPos.X) { return false; }
                }
            }

            return true;
        }
    }
}
