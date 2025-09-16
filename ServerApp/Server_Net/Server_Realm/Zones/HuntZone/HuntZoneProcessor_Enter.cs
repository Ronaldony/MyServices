using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 플레이어 스폰.
        /// </summary>
        private void EnterPlayer(WorldMsg_EnterPlayer worldMsg)
        {
            var pu = worldMsg.PlayerUnit;
            var section = pu.MovementHandler.Section;

            // Enter zone.
            if (false == EnterZone(pu, worldMsg))
            {
                return;
            }

            // 인근 섹션 플레이어, 적 정보 전달.
            var packetEnterPlayer = _memoryPoolService.Rent<GamePacketSC_EnterPlayer>();
            var aroundSections = GetAroundSections(section);

            foreach (var srcSection in aroundSections)
            {
                var ws = _worldGroup.WorldSectionDict[srcSection];

                // Players.
                foreach (var targetPU in ws.GetPlayerUnits())
                {
                    packetEnterPlayer.PlayInfos.Add(targetPU.UnitId, targetPU.PlayInfo);
                }

                // Mobs.
                foreach (var targetMU in ws.GetMobUnits())
                {
                    packetEnterPlayer.Mobs.Add(targetMU.UnitId, targetMU.Mob);
                }
            }

            packetEnterPlayer.UnitId = pu.UnitId;
            packetEnterPlayer.ZoneId = Global.HuntZoneId;

            // 진입 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(pu.Pid, GamePacketCode.SC_EnterPlayer, packetEnterPlayer);

            // Dispose.
            var packetSpawnUnits = _memoryPoolService.Rent<GamePacketSC_SpawnUnits>();

            // Add login player.
            packetSpawnUnits.PlayInfos.Add(pu.UnitId, pu.PlayInfo);

            // 패킷 전송 - 주변 플레이어에게 진입 유저 정보 전파.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_SpawnUnits, packetSpawnUnits);

            // Release pool.
            _memoryPoolService.Release(packetEnterPlayer);
            _memoryPoolService.Release(packetSpawnUnits);

            _logger.LogInformation($"Enter player. pid: {pu.Pid}");
        }

        /// <summary>
        /// Enter zone.
        /// </summary>
        private bool EnterZone(PlayerUnit pu, WorldMsg_EnterPlayer worldMsg)
        {
            if (_worldGroup.PlayerUnitDict.ContainsKey(pu.Pid))
            {
                return false;
            }

            var section = pu.MovementHandler.Section;

            // 월드에 플레이어 추가.
            _worldGroup.PlayerUnitDict.Add(pu.Pid, pu);
            _worldGroup.DataObjectDict.Add(pu.Pid, worldMsg.DataObjects);

            // Setup player unit.
            pu.ZoneProcessor = this;
            pu.WorldSection = _worldGroup.WorldSectionDict[section];
            pu.WorldSection.EnterPlayer(pu);

            return true;
        }

        /// <summary>
        /// 몹 스폰.
        /// </summary>
        private void SpawnMobs(WorldMsg_SpawnMob msg)
        {
            // key: 섹션, value: 스폰 유닛.
            var spawnMobDict = new Dictionary<SectionId, List<MobUnit>>();

            // Enemy 스폰.
            foreach (var section in msg.MobUnits.Keys)
            {
                var ws = _worldGroup.WorldSectionDict[section];

                // Enter into zone.
                foreach (var mu in msg.MobUnits[section])
                {
                    // Add mob.
                    _worldGroup.MobUnitDict.Add(mu.UnitId, mu);

                    // Setup.
                    mu.WorldSection = ws;
                    mu.WorldSection.EnterMob(mu);
                    mu.ZoneProcessor = this;
                }

                if (false == spawnMobDict.ContainsKey(section))
                {
                    spawnMobDict.Add(section, new List<MobUnit>());
                }

                // Add mobs.
                spawnMobDict[section].AddRange(msg.MobUnits[section]);
            }

            // 패킷.
            var spawnMobPacket = _memoryPoolService.Rent<GamePacketSC_SpawnMob>();

            foreach (var pair in spawnMobDict)
            {
                var ws = _worldGroup.WorldSectionDict[pair.Key];

                spawnMobPacket.Mobs = pair.Value.ToDictionary(d => d.UnitId, d => d.Mob);
                ws.SendPacket_AroundSections(GamePacketCode.SC_SpawnMob, spawnMobPacket);
            }

            _memoryPoolService.Release(spawnMobPacket);
        }
    }
}
