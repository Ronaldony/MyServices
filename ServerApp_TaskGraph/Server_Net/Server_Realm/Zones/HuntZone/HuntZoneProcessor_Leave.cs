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
        /// Leave player.
        /// </summary>
        private void LeavePlayer(WorldMsg_LeavePlayer msg)
        {
            _logger.LogWarning($"leave player. pid: {msg.Pid}");

            if (false == _worldGroup.PlayerUnitDict.TryGetValue(msg.Pid, out var pu))
            {
                _logger.LogError($"not exist player. pid: {msg.Pid}");
                return;
            }

            // 로그 아웃 패킷 전송.
            pu.WorldSection.SendPacket_AroundSections(GamePacketCode.SC_Leave, new GamePacketSC_Leave());

            // Remove player from distrcit.
            if (false == PostLeavePlayer(msg.Pid))
            {
                return;
            }

            _logger.LogWarning($"Log out end. pid: {msg.Pid}");
        }

        /// <summary>
        /// Post process liveaing player.
        /// </summary>
        private bool PostLeavePlayer(string pid)
        {
            // Leave zone.
            if (false == _worldGroup.PlayerUnitDict.Remove(pid, out var pu))
            {
                _logger.LogError($"remove player error. pid: {pu.Pid}, section: {pu.MovementHandler.Section}");
                return false;
            }

            // Leave section.
            if (null == pu.WorldSection.LeavePlayer(pu.UnitId))
            {
                _logger.LogError($"remove player from section. unitId: {pu.UnitId}, section: {pu.MovementHandler.Section}");
                return false;
            }

            // Player dispose.
            pu.Dispose();

            return true;
        }
    }
}
