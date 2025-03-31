using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;
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
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}"); 
                return;
            }

            var playInfo = worldPlayer.PlayInfo;

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(packet.PacketTime, playInfo.UpdateTime))
            {
                _logger.LogError($"Packe time error. packet: {packet.PacketTime.ToString("hh:mm:ss.fff")}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}");
                return;
            }

            // PlayInfo 갱신.
            playInfo.UpdateTime = packet.PacketTime;
            playInfo.Position.MoveDirection = packet.MoveDirection;

            // 근처 플레이어 세션 가져오기.
            var section = ((int)playInfo.Position.Pos.X / GameData.SECTION_WIDTH, (int)playInfo.Position.Pos.Y / GameData.SECTION_HEIGHT);
            var aroundSessions = GetAroundSessions(section);

            // 현재 플레이어 제외.
            aroundSessions.Remove(worldPlayer.GameSession);

            if (false == aroundSessions.Any())
            {
                return;
            }

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_MoveStart>(packet =>
            {
                packet.Pid = packet.Pid;

                var packetBytes = _dataSerializer.Serialize(packet);

                foreach (var otherSession in aroundSessions)
                {
                    SendPacket(otherSession.SessionId, otherSession.SessionUid, NetPacketCode.SC_MoveStart, packetBytes);
                }
            });

            // Log.
            //_logger.LogDebug($"MoveStart - pid: {packet.Pid}, pos: {playInfo.Position.Pos}, move: {packet.MoveDirection}, time: {packet.PacketTime.ToString("hh:mm:ss.fff")}");
        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveEnd(NetPacketCS_MoveEnd packet)
        {
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}"); 
                return;
            }

            // 플레이어 정보.
            var playInfo = worldPlayer.PlayInfo;
            var position = playInfo.Position;

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(packet.PacketTime, playInfo.UpdateTime))
            {
                _logger.LogError($"Packe time error. packet: {packet.PacketTime.ToString("hh:mm:ss.fff")}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}");
                SendMoveFixed(playInfo.Pid, position.Pos);
                return;
            }

            // 이동 거리 계산 및 유효성 검사.s
            var deltaTime = (packet.PacketTime - playInfo.UpdateTime).TotalSeconds;
            var isValidPosition = IsValidPosition(position.MoveDirection, deltaTime, playInfo.Stat.Velocity, position.Pos, packet.EndPos);

            //_logger.LogInformation($"MoveEnd - pid: {packet.Pid}, end pos: {packet.EndPos}, last pos: {playInfo.Position.Pos}, packet delta: {packet.DeltaTimeSec}, calc delta: {deltaTime}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}, packet time: {packet.PacketTime.ToString("hh:mm:ss.fff")}");

            if (false == isValidPosition)
            {
                _logger.LogError($"position error.");
                SendMoveFixed(playInfo.Pid, position.Pos);
                return;
            }

            // 업데이트 시간 및 좌표 갱신.
            playInfo.UpdateTime = packet.PacketTime;
            position.Pos = packet.EndPos;
            position.MoveDirection = Type_MoveDirection.None;

            // 근처 플레이어 세션 가져오기.
            var section = ((int)playInfo.Position.Pos.X / GameData.SECTION_WIDTH, (int)playInfo.Position.Pos.Y / GameData.SECTION_HEIGHT);
            var aroundSessions = GetAroundSessions(section);

            // 현재 플레이어 제외.
            aroundSessions.Remove(worldPlayer.GameSession);

            if (false == aroundSessions.Any())
            {
                return;
            }

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_MoveEnd>(packet =>
            {
                packet.Pid = packet.Pid;
                packet.EndPos = packet.EndPos;

                var packetBytes = _dataSerializer.Serialize(packet);

                foreach (var otherSession in aroundSessions)
                {
                    SendPacket(otherSession.SessionId, otherSession.SessionUid, NetPacketCode.SC_MoveEnd, packetBytes);
                }
            });
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidPosition(Type_MoveDirection moveDir, double diffSeconds, int velocity, Vector2 lastPos, Vector2 endPos)
        {
            // 이동 방향 계산 시퀀스.
            // 1. 이동 방향 확인.
            // 2. 이동 방향에 따라 이동 오차 범위 값 계산.
            // 3. x, y 좌표에 이동 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.

            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVENT_ERROR);

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MOVENT_ERROR) { return false; }
                        if (endPos.X + GameData.MOVENT_ERROR < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVENT_ERROR);

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MOVENT_ERROR) { return false; }
                        if (endPos.X + GameData.MOVENT_ERROR < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MOVENT_ERROR) { return false; }
                        if (endPos.Y + GameData.MOVENT_ERROR < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MOVENT_ERROR) { return false; }
                        if (endPos.Y + GameData.MOVENT_ERROR < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MOVENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MOVENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MOVENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity * 0.8 + GameData.MOVENT_ERROR);

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

        /// <summary>
        /// Send move fixed pakcet.
        /// </summary>
        private void SendMoveFixed(string pid, Vector2 fixedPos)
        {
            NetPacketUtility.SendPacket<NetPacketSC_MoveFixed>(sendPacket =>
            {
                sendPacket.Pid = pid;
                sendPacket.FixedPos = fixedPos;
                SendPacket(sendPacket.Pid, NetPacketCode.SC_MoveFixed, _dataSerializer.Serialize(sendPacket));
            });
        }
    }
}
