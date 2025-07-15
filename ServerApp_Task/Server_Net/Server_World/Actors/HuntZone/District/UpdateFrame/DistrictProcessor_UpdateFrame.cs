using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Section;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrame(float dt)
        {
            // 이동 처리.
            var updateFrames = new ConcurrentBag<World_UpdateFrame>();

            // 이동 업데이트.
            UpdateFrame_Move(dt, updateFrames);

            // 업데이트 프레임 적용.
            UpdateFrame_Apply(updateFrames);

            // Release memory.
            foreach (var updateFrame in updateFrames)
            {
                _memoryPoolService.Release(updateFrame.MoveSectionUnit.Values);
            }

            _memoryPoolService.Release(updateFrames.ToList());
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Apply(ConcurrentBag<World_UpdateFrame> updateFrames)
        {
            // 세션을 이동한 플레이어 정보 전달.
            foreach (var updateFrame in updateFrames)
            {
                // Apply - move section.
                UpdateFrame_ApplyMoveSection(updateFrame.MoveSectionUnit);

                // Apply - move district.
                UpdateFrame_ApplyMoveDistrict(updateFrame.MoveDistrictUnit);
            }
        }

        /// <summary>
        /// UpdateFrame_ApplyMoveSection.
        /// </summary>
        private void UpdateFrame_ApplyMoveSection(ConcurrentDictionary<SectionId, World_SectionUnit> moveSectionUnit)
        {
            // Rent.
            var updateSectionPacket = _memoryPoolService.Rent<NetPacketSC_UpdateSection>();

            // 섹션 이동 유닛 처리.
            foreach (var pair in moveSectionUnit)
            {
                var sectionUnit = pair.Value;

                if (false == sectionUnit.PlayInfoDict.Any()
                    && false == sectionUnit.Enemies.Any())
                {
                    continue;
                }

                if (sectionUnit.PlayInfoDict.Any())
                {
                    updateSectionPacket.PlayInfos.AddRange(sectionUnit.PlayInfoDict.Values);
                }

                if (sectionUnit.Enemies.Any())
                {
                    updateSectionPacket.Enemies.AddRange(sectionUnit.Enemies);
                }

                // Send packet.
                SendPacket_AroundSections(pair.Key, NetPacketCode.SC_UpdateSection, updateSectionPacket);

                // 섹션 이동 이벤트 라우팅.
                RouteGameEvent(new MoveSectionEvent
                {
                    RouteSection = pair.Key,
                    GameSessions = sectionUnit.PlayInfoDict.Keys
                });

                // clear.
                updateSectionPacket.PlayInfos.Clear();
                updateSectionPacket.Enemies.Clear();
            }

            // Release.
            _memoryPoolService.Release<NetPacketSC_UpdateSection>(updateSectionPacket);
        }

        /// <summary>
        /// UpdateFrame_ApplyMoveDistrict.
        /// </summary>
        private void UpdateFrame_ApplyMoveDistrict(ConcurrentDictionary<SectionId, World_DistrictUnit> moveDistrictUnit)
        {
            // 구역 이동 유닛 처리.
            foreach (var pair in moveDistrictUnit)
            {
                var districtUnit = pair.Value;

                if (false == districtUnit.WorldPlayers.Any()
                    && false == districtUnit.WorldEnemies.Any())
                {
                    continue;
                }

                // 구역 이동 이벤트 라우팅. 
                var moveDistrictEvent = new MoveDistrictEvent();

                moveDistrictEvent.RouteSection = pair.Key;
                moveDistrictEvent.FromDistrict = _district;

                // 구역 이동 이벤트 플레이어 추가
                // 현재 구역에서 플레이어 제거.
                foreach (var wp in districtUnit.WorldPlayers)
                {
                    moveDistrictEvent.WorldPlayers.Add(wp);
                    _worldGroup.PlayerDict.Remove(wp.Pid);
                    _logger.LogDebug($"remove player from district. pid: {wp.Pid}");
                }

                // 구역 이동 이벤트 적 추가
                // 현재 구역에서 적 제거.
                foreach (var we in districtUnit.WorldEnemies)
                {
                    moveDistrictEvent.WorldEnemies.Add(we);
                    _worldGroup.EnemyDict.Remove(we.EnemyUid);
                }

                _logger.LogDebug($"Move district. from {_district} to {pair.Key}");

                RouteGameEvent(moveDistrictEvent);
            }
        }
    }
}
