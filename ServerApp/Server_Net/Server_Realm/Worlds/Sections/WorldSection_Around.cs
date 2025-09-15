using System.Numerics;

namespace Server_Realm.Worlds.Sections
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// WorldSection.
    /// </summary>
    public partial class WorldSection
    {
        /// <summary>
        /// Detect around mobs.
        /// </summary>
        public IEnumerable<MobUnit> DetectAroundMobs()
        {
            return _mobUnitDict.Values.Concat(_aroundWorldSections.SelectMany(d => d.GetMobUnits())).ToList();
        }

        /// <summary>
        /// Detect around players.
        /// </summary>
        public IEnumerable<PlayerUnit> DetectAroundPlayers()
        {
            return _playerUnitDict.Values.Concat(_aroundWorldSections.SelectMany(d => d.GetPlayerUnits())).ToList();
        }

        /// <summary>
        /// Detect player from around.
        /// </summary>
        /// <param name="srcUnitId">Srouce unit id</param>
        /// <param name="detectRangeSqr">Detect range square</param>
        public PlayerUnit DetectPlayerAround(long srcUnitId)
        {
            // Find this section.
            var srcPos = _mobUnitDict[srcUnitId].MovementHandler.NowPos();

            foreach (var pu in _playerUnitDict.Values)
            {
                if (GameData.MOB_DETECT_RANGE_SQUARE >= Vector2.DistanceSquared(srcPos, pu.MovementHandler.NowPos()))
                {
                    // 플레이어 감지.
                    return pu;
                }
            }

            // Find around sections.
            foreach (var ws in _aroundWorldSections)
            {
                foreach (var pu in ws.GetPlayerUnits())
                {
                    if (GameData.MOB_DETECT_RANGE_SQUARE >= Vector2.DistanceSquared(srcPos, pu.MovementHandler.NowPos()))
                    {
                        // 플레이어 감지.
                        return pu;
                    }
                }
            }

            return null;
        }
    }
}
