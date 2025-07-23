
namespace Server_World.Zones.HuntZone.District
{
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
        private bool SendPacket<T>(string pid, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _huntZoneServer.Send(pid, packetCode, packetBytes);
        }

        /// <summary>
        /// Send packet to sessions.
        /// </summary>
        private void SendPacket<T>(ICollection<string> pids, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            foreach (var pid in pids)
            {
                _huntZoneServer.Send(pid, packetCode, packetBytes);
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(string ignorePid, SectionId section, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var wp in _worldGroup.PlayerDict.Values)
            {
                if (false == wp.Pid.Equals(ignorePid))
                {
                    _huntZoneServer.Send(wp.Pid, packetCode, packetBytes);
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
                _huntZoneServer.Send(wp.Pid, packetCode, packetBytes);
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
                _huntZoneServer.Send(wp.Pid, packetCode, packetBytes);
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
                        _huntZoneServer.Send(wp.Pid, packetEvent.PacketCode, packetEvent.PacketBytes);
                    }
                }
            }
        }
    }
}
