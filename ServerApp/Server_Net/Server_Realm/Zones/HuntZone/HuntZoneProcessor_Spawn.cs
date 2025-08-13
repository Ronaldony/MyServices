using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
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
        private void SpawnPlayer(WorldMsg_SpawnPlayer worldMsg)
        {
            _logger.LogDebug($"Spawn player in processor");
            var totalSw = new Stopwatch();
            totalSw.Start();

            var pu = worldMsg.PlayerUnit;
            var section = pu.MovementHandler.Section;

            // 월드에 플레이어 추가.
            _worldGroup.PlayerUnitDict.Add(pu.Pid, pu);

            // Setup player unit.
            pu.ZoneProcessor = this;
            pu.WorldSection = _worldGroup.WorldSectionDict[section];
            pu.WorldSection.EnterPlayer(pu);

            // 인근 섹션 플레이어, 적 정보 전달.
            var gamePacket = _memoryPoolService.Rent<GamePacketSC_SpawnPlayer>();
            var aroundSections = GetAroundSections(section);

            foreach (var srcSection in aroundSections)
            {
                var ws = _worldGroup.WorldSectionDict[srcSection];

                // Players.
                foreach (var targetPu in ws.GetPlayerUnits())
                {
                    gamePacket.PlayInfos.Add(targetPu.UnitId, targetPu.PlayInfo);
                }

                // Mobs.
                foreach (var targetWu in ws.GetMobUnits())
                {
                    gamePacket.Mobs.Add(targetWu.UnitId, targetWu.Mob);
                }
            }

            var packetSW1 = new Stopwatch();
            packetSW1.Start();
            // 진입 유저에게 인근 세션 유닛 정보 전송.
            SendPacket(pu.Pid, GamePacketCode.SC_SpawnPlayer, gamePacket);

            packetSW1.Stop();
            // Dispose.
            gamePacket.Dispose();

            // Add login player.
            gamePacket.PlayInfos.Add(pu.UnitId, pu.PlayInfo);

            var packetSW2 = new Stopwatch();
            packetSW2.Start();
            // 패킷 전송 - 주변 플레이어에게 진입 유저 정보 전파.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_SpawnPlayer, gamePacket);
            packetSW2.Stop();
            // Release pool.
            _memoryPoolService.Release(gamePacket);

            totalSw.Stop();
            _logger.LogDebug($"Spawn player end. pid: {pu.Pid}, total ms: {totalSw.ElapsedMilliseconds}, packet1 ms: {packetSW1.ElapsedMilliseconds}, packet2 ms: {packetSW2.ElapsedMilliseconds}");
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
                var worldSection = _worldGroup.WorldSectionDict[section];

                foreach (var mu in msg.MobUnits[section])
                {
                    // Enter.
                    _worldGroup.MobUnitDict.Add(mu.UnitId, mu);
                    mu.WorldSection = worldSection;
                    mu.WorldSection.EnterMob(mu);

                    // Setup.
                    mu.ZoneProcessor = this;
                }

                if (false == spawnMobDict.ContainsKey(section))
                {
                    spawnMobDict.Add(section, new List<MobUnit>());
                }

                // Spawn mob.
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
