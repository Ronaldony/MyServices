
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Numerics;

namespace Server_World.Actors.HuntZone.Game
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;

    /// <summary>
    /// GameProcessor.
    /// </summary>
    public partial class GameProcessor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveStart(World_Player worldPlayer, NetPacketCS_MoveStart packet)
        {
            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(packet.PacketTime, playInfo.UpdateTime))
            {
                _logger.LogError($"Packe time error. packet: {packet.PacketTime.ToString("hh:mm:ss.fff")}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}");
                return;
            }

            // PlayInfo 갱신.
            playInfo.Action = Type_Action.Move;
            playInfo.UpdateTime = packet.PacketTime;
            location.MoveDirection = packet.MoveDirection;

            // 패킷 전송 - 이동 시작.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession, 
                section: SectionId.GetSection(location.Position), 
                packetCode: NetPacketCode.SC_MoveStart, 
                packetBytes: _dataSerializer.Serialize(
                new NetPacketSC_MoveStart
                {
                    Pid = playInfo.Pid
                }));

            // Log.
            //_logger.LogDebug($"MoveStart - pid: {playInfo.Pid}, pos: {location.Position}, move: {packet.MoveDirection}, time: {packet.PacketTime.ToString("hh:mm:ss.fff")}");
        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void MoveEnd(World_Player worldPlayer, NetPacketCS_MoveEnd packet)
        {
            // 플레이어 정보.
            var playInfo = worldPlayer.PlayInfo;
            var location = playInfo.Location;
            var section = worldPlayer.Section;

            if (false == _router.IsValidSection(SectionId.GetSection(packet.EndPos)))
            {
                _logger.LogError($"Invalid section. section: {packet.EndPos}");
                return;
            }

            // 업데이트 시간보다 패킷 시간이 과거인 경우 패킷 무시.
            if (TimeUtil.IsPastTime(packet.PacketTime, playInfo.UpdateTime))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(
                    section: section,
                    packetCode: NetPacketCode.SC_MoveFixed,
                    packetBytes: _dataSerializer.Serialize(new NetPacketSC_MoveFixed
                    {
                        Pid = playInfo.Pid,
                        FixedPos = location.Position
                    }));

                _logger.LogError($"Packe time error. packet: {packet.PacketTime.ToString("hh:mm:ss.fff")}, update time: {playInfo.UpdateTime.ToString("hh:mm:ss.fff")}");
                return;
            }

            // 이동 거리 계산 및 유효성 검사.
            var deltaTime = (packet.PacketTime - playInfo.UpdateTime).TotalSeconds;

            if (false == IsValidPosition(location.MoveDirection, deltaTime, playInfo.Stat.Velocity, location.Position, packet.EndPos))
            {
                // 패킷 전송 - 이동 수정.
                SendPacket_AroundSections(
                    section: section,
                    packetCode: NetPacketCode.SC_MoveFixed,
                    packetBytes: _dataSerializer.Serialize(new NetPacketSC_MoveFixed
                    {
                        Pid = playInfo.Pid,
                        FixedPos = location.Position
                    }));

                _logger.LogError($"Move end position error. ");
                _logger.LogError($"ori: {location.Position}, dst: {packet.EndPos}, dir: {location.MoveDirection}");
                return;
            }

            // 업데이트 시간 및 좌표 갱신.
            playInfo.Action = Type_Action.MoveEnd;
            playInfo.UpdateTime = packet.PacketTime;
            location.PositionEnd = packet.EndPos;

            // 패킷 전송 - 이동 정지.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession,
                section: section,
                packetCode: NetPacketCode.SC_MoveEnd,
                packetBytes: _dataSerializer.Serialize(new NetPacketSC_MoveEnd
                {
                    Pid = playInfo.Pid,
                    EndPos = packet.EndPos
                }));
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
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MOVEMENT_ERROR) { return false; }
                        if (endPos.X + GameData.MOVEMENT_ERROR < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + GameData.MOVEMENT_ERROR) { return false; }
                        if (endPos.X + GameData.MOVEMENT_ERROR < lastPos.X) { return false; }
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                        if (endPos.Y + GameData.MOVEMENT_ERROR < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                        if (endPos.Y + GameData.MOVEMENT_ERROR < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X + distErrorRange < lastPos.X) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y > lastPos.Y + distErrorRange) { return false; }
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

                        // X 좌표 검사.
                        if (endPos.X > lastPos.X + distErrorRange) { return false; }

                        // Y 좌표 검사.
                        if (endPos.Y + distErrorRange < lastPos.Y) { return false; }
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        // 대각선 방향인 경우, 속도의 0.8배 만큼 x, y 좌표 계산.
                        var distErrorRange = diffSeconds * (velocity + GameData.MOVEMENT_ERROR);

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
