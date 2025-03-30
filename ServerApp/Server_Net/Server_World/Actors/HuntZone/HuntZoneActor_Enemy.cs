namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 스폰 적.
        /// </summary>
        private void SpawnEnemy(WorldMsg_SpawnEnemy msg)
        {
            // 적 스폰 병렬 처리.
            Parallel.ForEach(msg.Enemies, pair =>
            {
                var section = pair.Key;
                var enemies = pair.Value;

                // 섹션에 적 정보 추가.
                var enemyDict = _sectionEnemyDict[(section.sectionX, section.sectionY)];

                foreach (var enemy in enemies)
                {
                    enemyDict.Add(enemy.EnemeyUid, enemy);
                }

                var gameSessions = GetGameSessions(section.sectionX, section.sectionY);

                // 섹션에 플레이어가 없으면 스폰 무시.
                if (null == gameSessions)
                {
                    return;
                }

                // 각 플레이어에게 적 스폰 정보 전송.
                NetPacketUtility.SendPacket<NetPacketSC_SpawnEnemy>(packet =>
                {
                    packet.Enemies = enemies;

                    var pakcetBytes = _dataSerializer.Serialize(packet);

                    Parallel.ForEach(gameSessions, session =>
                    {
                        SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnEnemy, pakcetBytes);
                    });
                });
            });
        }
    }
}
