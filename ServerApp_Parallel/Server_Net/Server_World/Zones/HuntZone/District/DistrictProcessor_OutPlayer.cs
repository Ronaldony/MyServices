using Microsoft.Extensions.Logging;

namespace Server_World.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Out player.
        /// </summary>
        private void OutPlayer(WorldMsg_OutPlayer msg)
        {
            _logger.LogWarning($"Log out start. pid: {msg.Pid}");

            if (false == _worldGroup.PlayerDict.TryGetValue(msg.Pid, out var wp))
            {
                _logger.LogError($"not exist player. pid: {msg.Pid}");
                return;
            }

            // 로그 아웃 패킷 전송.
            SendPacket_AroundSections(wp.Section, GamePacketCode.SC_LogOut, new GamePacketSC_LogOut
            {
                Pid = msg.Pid,
            });

            // Remove player from distrcit.
            if (false == LogoutPlayer(msg.Pid))
            {
                return;
            }

            _logger.LogWarning($"Log out end. pid: {msg.Pid}");
        }

        /// <summary>
        /// Remove player from district.
        /// </summary>
        private bool LogoutPlayer(string pid)
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
            _collisionService.Release(wp.Collision);

            return true;
        }
    }
}
