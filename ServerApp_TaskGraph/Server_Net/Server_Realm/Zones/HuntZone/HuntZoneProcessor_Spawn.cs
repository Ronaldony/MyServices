using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 스폰 플레이어.
        /// </summary>
        private void SpawnPlayer(WorldMsg_SpawnPlayer worldMsg)
        {
            _logger.LogDebug($"Spawn plaeyr in processor");

            var pu = worldMsg.PlayerUnit;
            var section = pu.MovementHandler.Section;

            // 월드에 플레이어 추가.
            _worldGroup.PlayerUnitDict.Add(pu.UnitId, pu);
            _worldGroup.WorldSectionDict[section].EnterPlayer(pu);

            // 인근 섹션 플레이어, 적 정보 전달.
            var gamePacket = _memoryPoolService.Rent<GamePacketSC_SpawnPlayer>();
            var aroundSections = GetAroundSections(section);

            foreach (var targetSection in aroundSections)
            {
                // Players.
                foreach (var targetPu in _worldGroup.WorldSectionDict[section].GetPlayerUnits())
                {
                    gamePacket.PlayInfos.Add(targetPu.UnitId, targetPu.PlayInfo);
                }

                // Enemies.
                foreach (var targetWu in _worldGroup.WorldSectionDict[section].GetMobUnits())
                {
                    gamePacket.Mobs.Add(targetWu.UnitId, targetWu.Mob);
                }
            }

            // 로그인 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(pu.UnitId, GamePacketCode.SC_SpawnPlayer, gamePacket);

            // Dispose.
            gamePacket.Dispose();

            // Add login player.
            gamePacket.PlayInfos.Add(pu.UnitId, pu.PlayInfo);

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
            foreach (var section in msg.MobUnits.Keys)
            {
                // 섹션에 적 정보 추가.
                var worldSection = _worldGroup.WorldSectionDict[section];

                foreach (var mu in msg.MobUnits[section])
                {
                    // 몹 진입.
                    _worldGroup.MobUnitDict.Add(mu.UnitId, mu);
                    worldSection.EnterMob(mu);
                }

                if (false == spawnEnemyDict.ContainsKey(section))
                {
                    spawnEnemyDict.Add(section, new List<MobUnit>());
                }

                spawnEnemyDict[section].AddRange(msg.MobUnits[section]);
            }

            // 스폰할 적이 없는 경우 종료.
            if (false == spawnEnemyDict.Any())
            {
                return;
            }

            // 패킷.
            var gamePacket = new GamePacketSC_SpawnMob();

            foreach (var pair in spawnEnemyDict)
            {
                gamePacket.Mobs = pair.Value.ToDictionary(d => d.UnitId, d => d.Mob);
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_SpawnMob, gamePacket);
            }
        }
    }
}
