using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Collections.Concurrent;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.GameEvents;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Worlds.Section;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrame(WorldMsg_UpdateFrame worldMsg)
        {
            // 프레임 시간차.
            var nowTime = TimeUtil.Now;
            var frameDeltaTime = (nowTime - _lastFrameUpdateTime).TotalSeconds;

            // refersh frame time.
            _lastFrameUpdateTime = nowTime;

            // 이동 처리.
            var updateFrame = _memoryPoolService.Rent<World_UpdateFrame>();

            // 이동 업데이트.
            UpdateFrame_Move(frameDeltaTime, updateFrame);

            // 업데이트 프레임 적용.
            UpdateFrame_Apply(updateFrame);

            // Release memory.
            _memoryPoolService.Release(updateFrame.MoveSectionUnit.Values);
            _memoryPoolService.Release(updateFrame.MoveDistrictUnit.Values);
            _memoryPoolService.Release(updateFrame.HitSectionUnit.Values);
            _memoryPoolService.Release(updateFrame);
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Apply(World_UpdateFrame updateFrame)
        {
            // Apply - move section.
            UpdateFrame_ApplyMoveSection(updateFrame.MoveSectionUnit);

            // Apply - move district.
            UpdateFrame_ApplyMoveDistrict(updateFrame.MoveDistrictUnit);

            // Apply - projectiles.
            UpdateFrame_ApplyProjectile(updateFrame.MoveDistrictProjectile);

            // Apply - hit unit.
            UpdateFrame_ApplyHitUnit(updateFrame.HitSectionUnit);
        }

        /// <summary>
        /// 업데이트 프레임 적용 - 섹션 이동.
        /// </summary>
        private void UpdateFrame_ApplyMoveSection(ConcurrentDictionary<SectionId, World_SectionUnit> moveSectionUnit)
        {
            // Rent.
            var updateSectionPacket = _memoryPoolService.Rent<GamePacketSC_UpdateSectionMove>();

            // 섹션 이동 유닛 처리.
            foreach (var pair in moveSectionUnit)
            {
                var sectionUnit = pair.Value;

                if (false == sectionUnit.PlayInfoDict.Any() 
                    && false == sectionUnit.Enemies.Any())
                {
                    continue;
                }

                // 섹션 이동 유닛.
                updateSectionPacket.PlayInfos.AddRange(sectionUnit.PlayInfoDict.Values);
                updateSectionPacket.Enemies.AddRange(sectionUnit.Enemies);

                // Send packet.
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_UpdateSectionMove, updateSectionPacket);

                // 섹션 이동 이벤트 라우팅.
                RouteGameEvent(new MoveSectionEvent
                {
                    RouteSection = pair.Key,
                    MovePids = sectionUnit.PlayInfoDict.Keys
                });

                // clear.
                updateSectionPacket.PlayInfos.Clear();
                updateSectionPacket.Enemies.Clear();
            }

            // Release.
            _memoryPoolService.Release<GamePacketSC_UpdateSectionMove>(updateSectionPacket);
        }

        /// <summary>
        /// 업데이트 프레임 적용 - 구역 이동.
        /// </summary>
        private void UpdateFrame_ApplyMoveDistrict(ConcurrentDictionary<SectionId, World_DistrictUnit> moveDistrictUnit)
        {
            // 구역 이동 유닛 처리.
            foreach (var pair in moveDistrictUnit)
            {
                var districtUnit = pair.Value;

                if (false == districtUnit.PlayerEntities.Any()
                    && false == districtUnit.EnemyEntities.Any())
                {
                    continue;
                }

                // 구역 이동 이벤트 라우팅. 
                var moveDistrictEvent = new MoveDistrictEvent();

                moveDistrictEvent.RouteSection = pair.Key;
                moveDistrictEvent.FromDistrict = _district;

                // 구역 이동 유닛.
                moveDistrictEvent.PlayerEntities.AddRange(districtUnit.PlayerEntities);
                moveDistrictEvent.EnemyEntities.AddRange(districtUnit.EnemyEntities);

                // 구역 이동 이벤트 라우팅.
                RouteGameEvent(moveDistrictEvent);

                _logger.LogDebug($"Move district. from {_district} to {pair.Key}");
            }
        }

        /// <summary>
        /// 업데이트 프레임 적용 - 투사체.
        /// </summary>
        private void UpdateFrame_ApplyProjectile(ConcurrentDictionary<SectionId, World_DistrictProjectile> moveDistrictProjectile)
        {

        }

        /// <summary>
        /// 업데이트 프레임 적용 - 피격 유닛.
        /// </summary>
        private void UpdateFrame_ApplyHitUnit(ConcurrentDictionary<SectionId, World_SectionUnit> hitSectionUnit)
        {

        }
    }
}
