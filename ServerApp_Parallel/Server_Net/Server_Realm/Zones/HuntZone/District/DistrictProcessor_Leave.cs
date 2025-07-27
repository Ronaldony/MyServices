using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Out player.
        /// </summary>
        private void LeavePlayer(WorldMsg_LeavePlayer msg)
        {
            _logger.LogWarning($"leave player. pid: {msg.Pid}");

            if (false == _worldGroup.PlayerDict.TryGetValue(msg.Pid, out var pu))
            {
                _logger.LogError($"not exist player. pid: {msg.Pid}");
                return;
            }

            // 로그 아웃 패킷 전송.
            SendPacket_AroundSections(pu.Section, GamePacketCode.SC_Leave, new GamePacketSC_Leave
            {
                Pid = msg.Pid,
            });

            // Remove player from distrcit.
            if (false == PostLeavePlayer(msg.Pid))
            {
                return;
            }

            _logger.LogWarning($"Log out end. pid: {msg.Pid}");
        }

        /// <summary>
        /// Remove player from district.
        /// </summary>
        private bool PostLeavePlayer(string pid)
        {
            // Remove player.
            if (false == _worldGroup.PlayerDict.Remove(pid, out var wp))
            {
                _logger.LogError($"remove player error. pid: {pid}, section: {wp.Section}");
                return false;
            }

            // Remove player in section.
            if (false == _worldGroup.SectionPlayerDict[wp.Section].Remove(pid, out _))
            {
                _logger.LogError($"remove player from section. pid: {pid}, section: {wp.Section}");
                return false;
            }

            // Release.
            _collisionService.Release(wp.UnitCollision);

            return true;
        }
    }
}
