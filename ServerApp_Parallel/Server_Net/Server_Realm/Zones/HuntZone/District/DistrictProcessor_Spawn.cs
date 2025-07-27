using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;
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

            var pu = worldMsg.PlayerUnit;
            var section = pu.Section;

            // 관심 섹션인지 검사.
            if (false == IsManagedSection(section))
            {
                _logger.LogError($"not managed section. section: {pu.Section}");
                return;
            }

            // 플레이어 스폰..
            SpawnPlayerIntoZone(pu);

            // 인근 섹션 플레이어, 적 정보 전달.
            var gamePacket = _memoryPoolService.Rent<GamePacketSC_SpawnPlayer>();

            foreach (var targetSection in _worldGroup.ManagedSections)
            {
                // Players.
                foreach (var player in _worldGroup.SectionPlayerDict[section].Values)
                {
                    gamePacket.PlayInfos.Add(player.PlayInfo);
                }

                // Enemies.
                foreach (var we in _worldGroup.SectionMobDict[section].Values)
                {
                    gamePacket.Enemies.Add(we.Mob);
                }
            }

            // 로그인 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(pu.Pid, GamePacketCode.SC_SpawnPlayer, gamePacket);

            // Reset packet.
            gamePacket.Dispose();

            // Add login player.
            gamePacket.PlayInfos.Add(pu.PlayInfo);

            // 주변 섹션에 로그인 유저 정보 전송.
            SendPacket_AroundSections(pu.Pid, section, GamePacketCode.SC_SpawnPlayer, gamePacket);

            // Release pool.
            _memoryPoolService.Release(gamePacket);
        }

        /// <summary>
        /// 스폰 적.
        /// </summary>
        private void SpawnEnemy(WorldMsg_SpawnEnemy msg)
        {
            // key: 섹션, value: 스폰 유닛.
            var spawnEnemyDict = new Dictionary<SectionId, List<MobUnit>>();

            // Enemy 스폰.
            foreach (var section in _worldGroup.ManagedSections)
            {
                // 해당 섹션에 스폰할 적이 있는지 검사.
                if (false == msg.Enemies.ContainsKey(section))
                {
                    continue;
                }

                // 섹션에 적 정보 추가.
                var sectionEnemy = _worldGroup.SectionMobDict[section];

                foreach (var eu in msg.Enemies[section])
                {
                    // Add enemy into section.
                    sectionEnemy.AddOrUpdate(eu.UnitId, eu, (_, _) => eu);
                }

                if (false == spawnEnemyDict.ContainsKey(section))
                {
                    spawnEnemyDict.Add(section, new List<MobUnit>());
                }

                spawnEnemyDict[section].AddRange(msg.Enemies[section]);
            }

            // 스폰할 적이 없는 경우 종료.
            if (false == spawnEnemyDict.Any())
            {
                return;
            }

            // Send packet.
            var gamePacket = new GamePacketSC_SpawnEnemy();

            foreach (var pair in spawnEnemyDict)
            {
                gamePacket.Enemies = pair.Value.Select(d => d.Mob).ToList();
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_SpawnEnemy, gamePacket);
            }
        }

        /// <summary>
        /// 플레이어 스폰.
        /// </summary>
        private void SpawnPlayerIntoZone(PlayerUnit pu)
        {
            // 구역에 플레이어 추가.
            _worldGroup.SectionPlayerDict[pu.Section].AddOrUpdate(pu.Pid, pu, (_, _) => pu);
            _worldGroup.PlayerDict.Add(pu.Pid, pu);
        }
    }
}
