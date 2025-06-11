using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// 스폰 플레이어.
        /// </summary>
        private void SpawnPlayer(World_Player worldPlayer)
        {
            var session = worldPlayer.GameSession;

            // 세션 정보 가져오기.
            var section = GetSection(worldPlayer.PlayInfo.Location.Position);

            if (false == IsValidSection(section))
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
                var players = GetSectionPlayers(targetSection);
                if (null != players) { packet.Users.AddRange(players.Select(d => d.PlayInfo)); }

                // Enemies.
                var enemies = GetSectionEnemies(targetSection);
                if (null != enemies) { packet.Enemies.AddRange(enemies); }
            }

            // 인근 세션 적, 플레이어 정보 전송.
            SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(packet));

            // 근처 섹션 검색.
            var aroundPlayers = GetAroundPlayers(section);

            if (aroundPlayers.Any())
            {
                // Reset packet.
                packet.Users.Clear();
                packet.Enemies.Clear();

                // Add player info.
                packet.Users.Add(worldPlayer.PlayInfo);

                // 인근 세션에 패킷 전송.
                foreach (var player in aroundPlayers)
                {
                    // 다른 플레이어에게 패킷 전송.
                    SendPacket(player.GameSession.SessionId, player.GameSession.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(packet));
                }
            }

            // Release pool.
            _memoryPoolService.Release(packet);

            // 섹션에 플레이어 저장.
            if (false == AddPlayer(worldPlayer.PlayInfo.Pid, worldPlayer, section))
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

            // Enemy 스폰.
            foreach (var pair in msg.Enemies)
            {
                var section = pair.Key;
                var enemies = pair.Value;

                // 섹션에 적 정보 추가.
                var sectionEnemy = _sectionEnemyDict[section];

                // Add enemy into section.
                foreach (var enemy in enemies)
                {
                    // Add enemy.
                    _enemyDict.Add(enemy.EnemeyUid, enemy);

                    // Add enemy into section.
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, enemy, (_, _) => enemy);
                }

                // 근처 세션 가져오기.
                var aroundPlayers = GetAroundPlayers(section);

                // 플레이어 세션에 전송할 스폰 적 저장.
                foreach (var player in aroundPlayers)
                {
                    if (false == sessionEnemyDict.ContainsKey(player.GameSession))
                    {
                        sessionEnemyDict.Add(player.GameSession, new List<DTO_Enemy>());
                    }

                    sessionEnemyDict[player.GameSession].AddRange(enemies);
                }
            }

            // Send packet.
            var pakcet = new NetPacketSC_SpawnEnemy();

            foreach (var pair in sessionEnemyDict)
            {
                pakcet.Enemies = pair.Value;

                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_SpawnEnemy, _dataSerializer.Serialize(pakcet));
            }
        }
    }
}
