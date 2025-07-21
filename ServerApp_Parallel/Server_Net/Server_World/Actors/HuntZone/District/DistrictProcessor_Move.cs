
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
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveStart(World_Player worldPlayer, GamePacketCS_MoveStart packet)
        {
            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;

            // PlayInfo 갱신.
            playInfo.Action = Type_Action.Move;
            playInfo.PlayTime.LastPacketTime = packet.PacketTime;
            location.MoveDirection = packet.Direction;

            //_logger.LogDebug($"MoveStart - pos: {location.Position}, move: {packet.MoveDirection}");

            // 패킷 전송 - 이동 시작.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, GamePacketCode.SC_MoveStart, new GamePacketSC_MoveStart
            {
                Pid = worldPlayer.Pid,
                Direction = packet.Direction
            });
        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveEnd(World_Player worldPlayer, GamePacketCS_MoveEnd packet)
        {
            //_logger.LogDebug($"MoveEnd - pos: {packet.EndPos}");

            // 섹션 검증
            if (false == IsValidSection(packet.EndPos))
            {
                _logger.LogError($"Invalid section. section: {packet.EndPos}");
                return;
            }

            // 플레이어 정보.
            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;

            // 이동 거리 계산 및 유효성 검사.
            var deltaTime = (packet.PacketTime - playInfo.PlayTime.LastPacketTime).TotalSeconds + GameData.MOVEMENT_TIME_ERROR;

            if (false == IsValidPosition(deltaTime, playInfo.Stat.Velocity, location.MoveDirection, location.Position, packet.EndPos))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(worldPlayer.Section, GamePacketCode.SC_MoveFixed, new GamePacketSC_MoveFixed
                {
                    Pid = worldPlayer.Pid,
                    FixedPos = location.Position
                });

                _logger.LogError($"Move end position error. ");
                _logger.LogError($"ori: {location.Position}, dst: {packet.EndPos}, dir: {location.MoveDirection}");
                return;
            }

            // 업데이트 시간 및 좌표 갱신.
            playInfo.Action = Type_Action.MoveEnd;
            playInfo.PlayTime.LastPacketTime = packet.PacketTime;
            location.PositionEnd = packet.EndPos;

            // 패킷 전송 - 이동 정지.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, GamePacketCode.SC_MoveEnd, new GamePacketSC_MoveEnd
            {
                Pid = worldPlayer.Pid,
                EndPos = packet.EndPos
            });
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidPosition(double diffSeconds, int velocity, Vector2 direction, Vector2 lastPos, Vector2 endPos)
        {
            var dirY = direction.Y;
            var dirX = direction.X;

            // Y축 계산.
            if (dirY == 0)
            {
                // 고정축 계산.
                if (endPos.Y > lastPos.Y + GameData.MOVEMENT_FIXED_AXIS_ERROR) { return false; }
                if (endPos.Y + GameData.MOVEMENT_FIXED_AXIS_ERROR < lastPos.Y) { return false; }
            }
            else
            {
                var calcDist = Math.Abs(dirY * diffSeconds * velocity + GameData.MOVEMENT_ERROR);

                if (dirY > 0)
                {
                    // Top.
                    if (endPos.Y > lastPos.Y + calcDist) { return false; }
                }
                else
                {
                    // Down.
                    if (endPos.Y + calcDist < lastPos.Y) { return false; }
                }
            }

            // X축 계산.
            if (dirX == 0)
            {
                // 고정축 계산.
                if (endPos.X > lastPos.X + GameData.MOVEMENT_FIXED_AXIS_ERROR) { return false; }
                if (endPos.X + GameData.MOVEMENT_FIXED_AXIS_ERROR < lastPos.X) { return false; }
            }
            else
            {
                var calDist = Math.Abs(dirX * diffSeconds * velocity + GameData.MOVEMENT_ERROR);

                if (dirX > 0)
                {
                    // Right.
                    if (endPos.X > lastPos.X + calDist) { return false; }
                }
                else
                {
                    // Left.
                    if (endPos.X + calDist < lastPos.X) { return false; }
                }
            }

            return true;
        }
    }
}
