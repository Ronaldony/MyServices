using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
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
            // Leave zone.
            if (false == _worldGroup.PlayerDict.Remove(pid, out var pu))
            {
                _logger.LogError($"remove player error. pid: {pid}, section: {pu.Section}");
                return false;
            }

            // Leave section.
            if (null == _worldGroup.WorldSectionDict[pu.Section].LeavePlayer(pid))
            {
                _logger.LogError($"remove player from section. pid: {pid}, section: {pu.Section}");
                return false;
            }

            // Release.
            _collisionService.Release(pu.UnitCollision);

            return true;
        }
    }
}
