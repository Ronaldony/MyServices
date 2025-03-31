namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 적 이동 패턴 업데이트.
        /// </summary>
        private void UpdateMovePattern(WorldMsg_UpdateMovePattern msg)
        {
            var updateSessions = new Dictionary<GameSession, List<DTO_Enemy>>();

            // 이동 패턴 업데이트.
            foreach (var pair in _sectionEnemyDict)
            {
                var section = pair.Key;
                var enemies = pair.Value.Values;
                var updateEnemies = new List<DTO_Enemy>();

                foreach (var enemy in enemies)
                {
                    if (msg.MovePattern != enemy.Position.MovePattern)
                    {
                        continue;
                    }

                    // 다음 이동 패턴 업데이트.
                    var position = enemy.Position;
                    var index = position.RoamingPos.IndexOf(position.NextPos);

                    position.NextPos = position.RoamingPos[(index + 1) % position.RoamingPos.Count];

                    // 업데이트 대상 리스트업.
                    updateEnemies.Add(enemy);
                }

                // 업데이트 대상 세션에 추가.
                if (updateEnemies.Any())
                {
                    var aroundSessions = GetAroundSessions(section);

                    foreach (var session in aroundSessions)
                    {
                        if (false == updateSessions.ContainsKey(session))
                        {
                            updateSessions.Add(session, new List<DTO_Enemy>());
                        }

                        updateSessions[session].AddRange(updateEnemies);
                    }
                }
            }

            // 네트워크 전송.
            Parallel.ForEach(updateSessions, pair =>
            {
                NetPacketUtility.SendPacket<NetPacketSC_UpdateMovePattern>(packet =>
                {
                    packet.Enemies = pair.Value;

                    var packetBytes = _dataSerializer.Serialize(packet);

                    SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_UpdateMovePattern, packetBytes);
                });
            });
        }
    }
}
