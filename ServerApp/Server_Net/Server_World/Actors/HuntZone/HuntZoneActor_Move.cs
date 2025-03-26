using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using ServerEngine.Core.Util;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveStart(NetPacketCS_MoveStart packet)
        {
            var playInfo = GetPlayInfo(packet.Pid);

            if (playInfo == null)
            {
                _logger.LogError($"not exist pid. {packet.Pid}");
                return;
            }

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(playInfo.UpdateTime, packet.PacketTime))
            {
                _logger.LogError($"Packe time error. packet: {packet.PacketTime}, update time: {playInfo.UpdateTime}");
                return;
            }

            // 업데이트 시간 갱신.
            playInfo.UpdateTime = packet.PacketTime;

            // 이동 방향 저장.
            playInfo.Position.MoveDirection = packet.MoveDirection;

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_MoveStart>(pakcet =>
            {
                pakcet.Pid = packet.Pid;
                SendPacket(packet.Pid, NetPacketCode.SC_MoveStart, _dataSerializer.Serialize(pakcet));
            });

            // Log.
            _logger.LogDebug($"MoveStart - pid: {packet.Pid}, pos: {playInfo.Position.Pos}, move: {packet.MoveDirection}, time: {packet.PacketTime}");
        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveEnd(NetPacketCS_MoveEnd packet)
        {
            var playInfo = GetPlayInfo(packet.Pid);

            if (playInfo == null)
            {
                _logger.LogError($"not exist pid. {packet.Pid}");
                return;
            }

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(playInfo.UpdateTime, packet.PacketTime.AddSeconds(1)))
            {
                _logger.LogError($"Packe time error. packet: {packet.PacketTime}, update time: {playInfo.UpdateTime}");
                return;
            }

            // 이동 거리 계산 및 유효성 검사.
            var isValidPosition = IsValidPosition(packet.DeltaTimeSec, playInfo.Position.MoveDirection, playInfo.Stat.Velocity, playInfo.Position.Pos, packet.EndPos);

            _logger.LogInformation($"MoveEnd - pid: {packet.Pid}, end pos: {packet.EndPos}, last pos: {playInfo.Position.Pos} time: {packet.PacketTime}");

            if (false == isValidPosition)
            {
                _logger.LogError($"position error.");
                return;
            }

            // 업데이트 시간 및 좌표 갱신.
            playInfo.UpdateTime = packet.PacketTime;
            playInfo.Position.Pos = packet.EndPos;

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_MoveStart>(pakcet =>
            {
                pakcet.Pid = packet.Pid;
                SendPacket(packet.Pid, NetPacketCode.SC_MoveEnd, _dataSerializer.Serialize(pakcet));
            });
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidPosition(float diffSeconds, Type_MoveDirection moveDir, int velocity, Vector2 lastPos, Vector2 endPos)
        {
            // 이동 방향 계산 시퀀스.
            // 1. 이동 방향 확인.
            // 2. 이동 방향에 따라 이동 오차 범위 값 계산.
            // 3. x, y 좌표에 이동 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.

            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MovementError);

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MovementError) { return false; }
                        if (endPos.X + GameData.MovementError < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MovementError);

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MovementError) { return false; }
                        if (endPos.X + GameData.MovementError < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MovementError) { return false; }
                        if (endPos.Y + GameData.MovementError < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MovementError) { return false; }
                        if (endPos.Y + GameData.MovementError < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.71배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.71배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.71배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.71배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MovementError);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
