using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 스폰 플레이어.
        /// </summary>
        private void SpawnPlayer(WorldMsg_SpawnPlayer worldMsg)
        {
            var wp = worldMsg.WorldPlayer;
            var session = wp.GameSession;

            // 세션 정보 가져오기.
            var section = SectionId.GetSection(wp.PlayInfo.Location.Position);

            // 섹션에 플레이어 저장.
            if (false == AddPlayer(wp.PlayInfo.Pid, wp, section))
            {
                return;
            }

            if (false == IsManagedSection(section))
            {
                _logger.LogError($"not managed section. section: {section}");
                return;
            }

            // Section 갱신.
            wp.Section = section;

            // 인근 섹션 플레이어, 적 정보 전달.
            var netPacket = _memoryPoolService.Rent<NetPacketSC_SpawnPlayer>();

            foreach (var targetSection in _worldGroup.ManagedSections)
            {
                // Player.
                var players = _worldGroup.SectionPlayerDict[section].Values;

                foreach (var player in players)
                {
                    netPacket.PlayInfos.Add(player.PlayInfo);
                }

                // Enemies.
                var enemies = _worldGroup.SectionEnemyDict[section].Values;

                foreach (var enemy in enemies)
                {
                    netPacket.Enemies.Add(enemy.Enemy);
                }
            }

            // 로그인 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, netPacket);

            // Reset packet.
            netPacket.PlayInfos.Clear();
            netPacket.Enemies.Clear();

            // Add login player.
            netPacket.PlayInfos.Add(wp.PlayInfo);

            // 주변 섹션에 로그인 유저 정보 전송.
            SendPacket_AroundSections(wp.GameSession, section, NetPacketCode.SC_SpawnPlayer, netPacket);

            // Release pool.
            _memoryPoolService.Release(netPacket);
        }

        /// <summary>
        /// 스폰 적.
        /// </summary>
        private void SpawnEnemy(WorldMsg_SpawnEnemy msg)
        {
            // key: 섹션에 있는 세션, value: 스폰 적 유닛.
            var spawnEnemyDict = new Dictionary<SectionId, List<DTO_Enemy>>();

            // Enemy 스폰.
            foreach (var section in _worldGroup.ManagedSections)
            {
                // 해당 섹션에 스폰할 적이 있는지 검사.
                if (false == msg.Enemies.ContainsKey(section))
                {
                    continue;
                }

                // 섹션에 적 정보 추가.
                var sectionEnemy = _worldGroup.SectionEnemyDict[section];

                foreach (var enemy in msg.Enemies[section])
                {
                    var worldEnemy = new World_Enemy
                    {
                        Enemy = enemy,
                        CollisionBox = _collisionService.Rent(Type_Collision.Enemy)
                    };

                    // Add enemy.
                    _worldGroup.EnemyDict.Add(enemy.EnemeyUid, worldEnemy);

                    // Add enemy into section.
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, worldEnemy, (_, _) => worldEnemy);
                }

                if (false == spawnEnemyDict.ContainsKey(section))
                {
                    spawnEnemyDict.Add(section, new List<DTO_Enemy>());
                }

                // 스폰 대상 추가.
                spawnEnemyDict[section].AddRange(msg.Enemies[section]);
            }

            // 스폰할 적이 없는 경우 종료.
            if (false == spawnEnemyDict.Any())
            {
                return;
            }

            // Send packet.
            var netPacket = new NetPacketSC_SpawnEnemy();

            foreach (var pair in spawnEnemyDict)
            {
                netPacket.Enemies = pair.Value;
                SendPacket_AroundSections(pair.Key, NetPacketCode.SC_SpawnEnemy, netPacket);
            }
        }

        /// <summary>
        /// Add player to section.
        /// </summary>
        private bool AddPlayer(string pid, World_Player player, SectionId section)
        {
            // 플레이어 추가 - 섹션
            if (false == _worldGroup.SectionPlayerDict[section].TryAdd(pid, player))
            {
                return false;
            }

            // 플레이어 추가.
            if (false == _worldGroup.PlayerDict.TryAdd(pid, player))
            {
                return false;
            }

            return true;
        }
    }
}
