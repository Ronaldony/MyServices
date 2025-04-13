using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Worlds;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 스폰 플레이어.
        /// </summary>
        private void SpawnPlayer(World_Player player)
        {
            // 섹션.
            var session = player.GameSession;

            // 세션 정보 가져오기.
            if (false == IsValidSection((int)player.PlayInfo.Position.Pos.X, (int)player.PlayInfo.Position.Pos.Y, out var section))
            {
                _logger.LogError($"Invalid section.");
                return;
            }
            
            // 인근 섹션 플레이어, 적 정보 전달.
            var aroundSections = GetAroundSections(section);
            var packet = _memoryPoolService.Rent<NetPacketSC_SpawnPlayer>();

            packet.Users.Clear();
            packet.Enemies.Clear();

            foreach (var targetSection in aroundSections)
            {
                // Player.
                var players = GetWorldPlayers(targetSection);
                if (null != players) { packet.Users.AddRange(players.Select(d => d.PlayInfo)); }

                // Enemies.
                var enemies = GetEnemies(targetSection);
                if (null != enemies) { packet.Enemies.AddRange(enemies); }
            }

            // 인근 세션 적, 플레이어 정보 전송.
            SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(packet));

            // 근처 섹션 검색.s
            var aroundSessions = GetAroundSessions(section);

            if (aroundSessions.Any())
            {
                packet.Enemies.Clear();
                packet.Users.Clear();
                packet.Users.Add(player.PlayInfo);

                // 인근 세션에 패킷 전송.
                foreach (var otherSession in aroundSessions)
                {
                    // 다른 플레이어에게 패킷 전송.
                    SendPacket(otherSession.SessionId, otherSession.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(packet));
                }
            }

            // Release pool.
            _memoryPoolService.Release(packet);

            // 섹션에 플레이어 저장.
            if (false == AddPlayer(player.PlayInfo.Pid, player, section))
            {
                return;
            }
        }

        /// <summary>
        /// 스폰 적.
        /// </summary>
        private void SpawnEnemy(WorldMsg_SpawnEnemy msg)
        {
            var sessionEnemyDict = new Dictionary<GameSession, List<DTO_Enemy>>();

            // 적 스폰 병렬 처리.
            foreach (var pair in msg.Enemies)
            {
                var section = pair.Key;
                var enemies = pair.Value;

                // 섹션에 적 정보 추가.
                var sectionEnemy = _sectionEnemyDict[section];

                foreach (var enemy in enemies)
                {
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, enemy, (_, _) => enemy);
                }

                // 근처 세션 가져오기.
                var aroundSessions = GetAroundSessions(section);

                // 플레이어 세션에 전송할 스폰 적 저장.
                foreach (var session in aroundSessions)
                {
                    if (false == sessionEnemyDict.ContainsKey(session))
                    {
                        sessionEnemyDict.Add(session, new List<DTO_Enemy>());
                    }

                    sessionEnemyDict[session].AddRange(enemies);
                }
            }

            // send packet.
            if (false == sessionEnemyDict.Any())
            {
                return;
            }


            foreach (var pair in sessionEnemyDict)
            {
                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_SpawnEnemy, _dataSerializer.Serialize(new NetPacketSC_SpawnEnemy
                {
                    Enemies = pair.Value
                }));
            }

            // Check spawn count.
            _enemyCount += msg.Enemies.Count();

            if (_enemyCount >= GameData.MAX_ENEMY_SPAWN)
            {
                _schedulerEnemy.StopSpawn();
            }
        }
    }
}
