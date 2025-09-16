using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Leave player.
        /// </summary>
        private void LeavePlayer(WorldMsg_LeavePlayer msg)
        {
            _logger.LogInformation($"leave player. pid: {msg.Pid}");

            if (false == _worldGroup.PlayerUnitDict.TryGetValue(msg.Pid, out var pu))
            {
                return;
            }

            // Get WorldSection.
            var ws = pu.WorldSection;

            // Remove player from distrcit.
            if (false == LeaveZone(msg.Pid))
            {
                _logger.LogError($"fail to post leave player");
                return;
            }

            // 로그 아웃 패킷 전송.
            ws.SendPacket_AroundSections(GamePacketCode.SC_Leave, new GamePacketSC_Leave
            {
                UnitId = pu.UnitId
            });

            // Collect reusable unit.
            CollectReusableUnit(pu);

            _logger.LogWarning($"Leave player. pid: {msg.Pid}");
        }

        /// <summary>
        /// Post process liveaing player.
        /// </summary>
        private bool LeaveZone(string pid)
        {
            // Leave zone.
            _worldGroup.DataObjectDict.Remove(pid);
            if (false == _worldGroup.PlayerUnitDict.Remove(pid, out var pu))
            {
                _logger.LogError($"remove player error. pid: {pid}");
                return false;
            }

            // Leave section.
            pu.WorldSection.LeavePlayer(pu.UnitId);
            pu.Dispose();

            // Enter zone router.
            // TODO: MessageBus에서 제거.
            //_zoneRouter.LeavePlayer(pu.Pid);

            return true;
        }
    }
}
