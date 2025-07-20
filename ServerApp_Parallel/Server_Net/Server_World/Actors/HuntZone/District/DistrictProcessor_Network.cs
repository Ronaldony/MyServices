
namespace Server_World.Actors.HuntZone.District
{
    using Framework.Core.Session;
    using GameCore.GamePacket;
    using Server_World.Core.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Send packet.
        /// </summary>
        private bool SendPacket<T>(int sessionId, ulong sessionUid, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _huntZoneServer.Send(sessionId, sessionUid, packetCode, packetBytes);
        }

        /// <summary>
        /// Send packet to sessions.
        /// </summary>
        private void SendPacket<T>(ICollection<GameSession> sessions, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            foreach (var session in sessions)
            {
                _huntZoneServer.Send(session.SessionId, session.SessionUid, packetCode, packetBytes);
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(GameSession ignoreSession, SectionId section, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                var session = wp.GameSession;

                if (false == session.Equals(ignoreSession))
                {
                    _huntZoneServer.Send(session.SessionId, session.SessionUid, packetCode, packetBytes);
                }
            }

            // Broadcast packet around.
            _router.BroadcastPacketAround(new BroadcastPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(SectionId section, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                _huntZoneServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetCode, packetBytes);
            }

            // Broadcast packet around.
            _router.BroadcastPacketAround(new BroadcastPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for managed sections.
        /// </summary>
        private void SendPacket_ManagedSections<T>(int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                _huntZoneServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetCode, packetBytes);
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundFromEvent(BroadcastPacketEvent packetEvent)
        {
            var aroundSections = _router.GetAroundSections(packetEvent.RouteSection);

            // 관리 섹션중 패킷 발생지 주변인 섹션에 패킷 발송.
            foreach (var section in _worldGroup.ManagedSections)
            {
                if (aroundSections.Contains(section))
                {
                    foreach (var wp in _worldGroup.SectionPlayerDict[section].Values)
                    {
                        _huntZoneServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetEvent.PacketCode, packetEvent.PacketBytes);
                    }
                }
            }
        }
    }
}
