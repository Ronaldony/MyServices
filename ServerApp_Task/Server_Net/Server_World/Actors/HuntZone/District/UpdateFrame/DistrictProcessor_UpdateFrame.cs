using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;

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

            // 세션 처리.
            UpdateFrame_Session(updateFrames);

            // Release memory.
            foreach (var updateFrame in updateFrames)
            {
                _memoryPoolService.Release(updateFrame.MoveSectionUnit.Values);
            }

            _memoryPoolService.Release(updateFrames.ToList());
        }

        /// <summary>
        /// 업데이트 프레임 - 세션.
        /// </summary>
        private void UpdateFrame_Session(ConcurrentBag<World_UpdateFrame> updateFrames)
        {
            // 세션을 이동한 플레이어 정보 전달.
            foreach (var updateFrame in updateFrames)
            {
                // Rent.
                var updateSectionPacket = _memoryPoolService.Rent<NetPacketSC_UpdateSection>();

                // 섹션 이동 유닛 처리.
                foreach (var pair in updateFrame.MoveSectionUnit)
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

                // 구역 이동 유닛 처리.
                foreach (var pair in updateFrame.MoveDistrictUnit)
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
}
