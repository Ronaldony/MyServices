
namespace Server_World.Actors.HuntZone.District
{
    using Consul;
    using Framework.Core.Session;
    using GameCore.NetPacket;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Send packet.
        /// </summary>
        private bool SendPacket<T>(int sessionId, ulong sessionUid, int packetCode, T packet)
            where T : NetPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _worldServer.Send(sessionId, sessionUid, packetCode, packetBytes);
        }

        /// <summary>
        /// Send packet to sessions.
        /// </summary>
        private void SendPacket<T>(ICollection<GameSession> sessions, int packetCode, T packet)
            where T : NetPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            foreach (var session in sessions)
            {
                _worldServer.Send(session.SessionId, session.SessionUid, packetCode, packetBytes);
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(GameSession ignoreSession, SectionId section, int packetCode, T packet)
            where T : NetPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                var session = wp.GameSession;

                if (false == session.Equals(ignoreSession))
                {
                    _worldServer.Send(session.SessionId, session.SessionUid, packetCode, packetBytes);
                }
            }

            // Broadcast packet around.
            _router.BroadcastPacketAround(new BroadcastPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(SectionId section, int packetCode, T packet)
            where T : NetPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                _worldServer.Send(wp.GameSession.SessionId, wp.GameSession.SessionUid, packetCode, packetBytes);
            }

            // Broadcast packet around.
            _router.BroadcastPacketAround(new BroadcastPacketEvent(section, packetCode, packetBytes));
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundFromEvent(BroadcastPacketEvent packetEvent)
        {
            var aroundSections = _router.GetAroundSections(packetEvent.FromSection);

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
