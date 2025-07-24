using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;

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

            var wp = worldMsg.PlayerUnit;

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
            var GamePacket = _memoryPoolService.Rent<GamePacketSC_SpawnPlayer>();

            foreach (var targetSection in _worldGroup.ManagedSections)
            {
                // Players.
                foreach (var player in _worldGroup.SectionPlayerDict[section].Values)
                {
                    GamePacket.PlayInfos.Add(player.PlayInfo);
                }

                // Enemies.
                foreach (var we in _worldGroup.SectionEnemyDict[section].Values)
                {
                    GamePacket.Enemies.Add(we.Enemy);
                }
            }

            // 로그인 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(wp.Pid, GamePacketCode.SC_SpawnPlayer, GamePacket);

            // Reset packet.
            GamePacket.PlayInfos.Clear();
            GamePacket.Enemies.Clear();

            // Add login player.
            GamePacket.PlayInfos.Add(wp.PlayInfo);

            // 주변 섹션에 로그인 유저 정보 전송.
            SendPacket_AroundSections(wp.Pid, section, GamePacketCode.SC_SpawnPlayer, GamePacket);

            // Release pool.
            _memoryPoolService.Release(GamePacket);
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
                    var ee = new EnemyUnit
                    {
                        Enemy = enemy,
                        EnemyUid = enemy.EnemeyUid,
                        Section = SectionId.GetSection(enemy.Location.Pos),
                        Collision = _collisionService.Rent(Type_Collision.Enemy)
                    };

                    // Add enemy into section.
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, ee, (_, _) => ee);
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
            var GamePacket = new GamePacketSC_SpawnEnemy();

            foreach (var pair in spawnEnemyDict)
            {
                GamePacket.Enemies = pair.Value;
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_SpawnEnemy, GamePacket);
            }
        }

        /// <summary>
        /// Add player into district.
        /// </summary>
        private void AddPlayerIntoDistrict(PlayerUnit pe)
        {
            // 구역에 플레이어 추가.
            _worldGroup.SectionPlayerDict[pe.Section].AddOrUpdate(pe.Pid, pe, (_, _) => pe);
            _worldGroup.PlayerDict.Add(pe.Pid, pe);
        }
    }
}
