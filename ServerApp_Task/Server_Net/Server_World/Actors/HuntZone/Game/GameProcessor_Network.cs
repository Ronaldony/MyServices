
namespace Server_World.Actors.HuntZone.Game
{
    using Framework.Core.Session;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// GameProcessor.
    /// </summary>
    public partial class GameProcessor
    {
        /// <summary>
        /// Get session.
        /// </summary>
        private GameSession GetSession(string pid)
        {
            if (_worldGroup.PlayerDict.TryGetValue(pid, out var player))
            {
                return player.GameSession;
            }

            return null;
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        private void SendPacket(string pid, int packetCode, byte[] packetData)
        {
            if (false == _worldGroup.PlayerDict.TryGetValue(pid, out var player))
            {
                return;
            }

            // Send.
            _worldServer.Send(player.GameSession.SessionId, player.GameSession.SessionUid, packetCode, packetData);
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        private bool SendPacket(int sessionId, ulong sessionUid, int packetCode, byte[] packetData)
        {
            // Send.
            return _worldServer.Send(sessionId, sessionUid, packetCode, packetData);
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections(GameSession ignoreSession, SectionId section, int packetCode, byte[] packetBytes)
        {
            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                var session = wp.GameSession;

                if (false == session.Equals(ignoreSession))
                {
                    _worldServer.Send(session.SessionId, session.SessionUid, packetCode, packetBytes);
                }
            }

            // Route pakcet.
            _router.RoutePacketAroundSections(new AroundPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections(SectionId section, int packetCode, byte[] packetBytes)
        {
            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                _worldServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetCode, packetBytes);
            }

            // Route pakcet.
            _router.RoutePacketAroundSections(new AroundPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundFromEvent(AroundPacketEvent packetEvent)
        {
            var aroundSections = _router.GetAroundSections(packetEvent.Section);

            // 관리 섹션중 패킷 발생지 주변인 섹션에 패킷 발송.
            foreach (var section in _worldGroup.ManagedSections)
            {
                if (aroundSections.Contains(section))
                {
                    foreach (var wp in _worldGroup.SectionPlayerDict[section].Values)
                    {
                        _worldServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetEvent.PacketCode, packetEvent.PacketBytes);
                    }
                }
            }
        }
    }
}
