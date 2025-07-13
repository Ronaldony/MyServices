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
            _logger.LogDebug($"Spawn plaeyr in processor");

            var wp = worldMsg.WorldPlayer;
            var session = wp.GameSession;

            // 세션 정보 가져오기.
            var section = wp.Section;

            if (false == IsManagedSection(wp.Section))
            {
                _logger.LogError($"not managed section. section: {wp.Section}");
                return;
            }

            // Add player into district.
            AddPlayerIntoDistrict(wp);

            // 인근 섹션 플레이어, 적 정보 전달.
            var netPacket = _memoryPoolService.Rent<NetPacketSC_SpawnPlayer>();

            foreach (var targetSection in _worldGroup.ManagedSections)
            {
                // Players.
                foreach (var player in _worldGroup.SectionPlayerDict[section].Values)
                {
                    netPacket.PlayInfos.Add(player.PlayInfo);
                }

                // Enemies.
                foreach (var we in _worldGroup.SectionEnemyDict[section].Values)
                {
                    netPacket.Enemies.Add(we.Enemy);
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
                    var we = new World_Enemy
                    {
                        Enemy = enemy,
                        EnemyUid = enemy.EnemeyUid,
                        Section = SectionId.GetSection(enemy.Location.Pos),
                        CollisionBox = _collisionService.Rent(Type_Collision.Enemy)
                    };

                    // Add enemy.
                    _worldGroup.EnemyDict.Add(enemy.EnemeyUid, we);

                    // Add enemy into section.
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, we, (_, _) => we);
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
        /// Add player into district.
        /// </summary>
        private void AddPlayerIntoDistrict(World_Player wp)
        {
            // 구역에 플레이어 추가.
            _worldGroup.SectionPlayerDict[wp.Section].AddOrUpdate(wp.Pid, wp, (_, _) => wp);
            _worldGroup.PlayerDict.Add(wp.Pid, wp);
        }
    }
}
