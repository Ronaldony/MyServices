
namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
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
        private bool SendPacket(string pid, int packetCode, byte[] packetData)
        {
            if (false == _worldGroup.PlayerDict.TryGetValue(pid, out var player))
            {
                return false;
            }

            // Send.
            return _worldServer.Send(player.GameSession.SessionId, player.GameSession.SessionUid, packetCode, packetData);
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
        private void SendPacket_AroundSections(GameSession ignoreSession, (int x, int y) section, int packetCode, byte[] packetBytes)
        {
            // 섹션.
            var aroundPlayers = GetAroundPlayers(section);

            foreach (var player in aroundPlayers)
            {
                // 현재 플레이어 제외.
                if (player.GameSession.Equals(ignoreSession)) { continue; }

                _worldServer.Send(player.GameSession.SessionId, player.GameSession.SessionUid, packetCode, packetBytes);
            }
        }
    }
}
