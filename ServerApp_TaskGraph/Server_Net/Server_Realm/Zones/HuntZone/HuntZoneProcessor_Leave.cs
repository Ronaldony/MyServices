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
            _logger.LogWarning($"leave player. unitID: {msg.UnitId}");

            if (false == _worldGroup.UnitDict.TryGetValue(msg.UnitId, out var pu))
            {
                _logger.LogError($"not exist player. unitId: {msg.UnitId}");
                return;
            }

            // 로그 아웃 패킷 전송.
            SendPacket_AroundSections(pu.Section, GamePacketCode.SC_Leave, new GamePacketSC_Leave());

            // Remove player from distrcit.
            if (false == PostLeavePlayer(msg.UnitId))
            {
                return;
            }

            _logger.LogWarning($"Log out end. unitId: {msg.UnitId}");
        }

        /// <summary>
        /// Remove player from district.
        /// </summary>
        private bool PostLeavePlayer(long unitId)
        {
            // Leave zone.
            if (false == _worldGroup.UnitDict.Remove(unitId, out var pu))
            {
                _logger.LogError($"remove player error. unitId: {unitId}, section: {pu.Section}");
                return false;
            }

            // Leave section.
            if (null == _worldGroup.WorldSectionDict[pu.Section].LeavePlayer(pu.UnitId))
            {
                _logger.LogError($"remove player from section. unitId: {unitId}, section: {pu.Section}");
                return false;
            }

            // Release.
            _collisionService.Return(pu.UnitCollision);

            return true;
        }
    }
}
