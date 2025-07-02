using System.Collections.Concurrent;

namespace TestProject.Datas
{
    // ===== 3. 공간 분할 관리자 (Thread-Safe) =====
    /// <summary>
    /// SpatialManager.
    /// </summary>
    public partial class SpatialManager
    {
        private readonly ConcurrentDictionary<SectionId, ConcurrentHashSet<int>> _sectionPlayers = new();
        private readonly ConcurrentDictionary<int, SectionId> _playerSections = new();
        private readonly float _sectionSize;

        public SpatialManager(float sectionSize = 100.0f)
        {
            _sectionSize = sectionSize;
        }

        public SectionId GetSection(Vector3 position) => new(
            (int)Math.Floor(position.X / _sectionSize),
            (int)Math.Floor(position.Z / _sectionSize)
        );

        public async Task<bool> MovePlayerToSectionAsync(int playerId, SectionId newSection)
        {
            if (_playerSections.TryGetValue(playerId, out var oldSection))
            {
                if (oldSection == newSection) return false;

                // 이전 섹션에서 제거
                if (_sectionPlayers.TryGetValue(oldSection, out var oldSet))
                {
                    oldSet.TryRemove(playerId);
                }
            }

            // 새 섹션에 추가
            var newSet = _sectionPlayers.GetOrAdd(newSection, _ => new ConcurrentHashSet<int>());
            newSet.Add(playerId);
            _playerSections[playerId] = newSection;

            return true;
        }

        public IEnumerable<int> GetPlayersInSection(SectionId section)
        {
            return _sectionPlayers.TryGetValue(section, out var players) ? players.ToArray() : Array.Empty<int>();
        }

        public IEnumerable<int> GetNearbyPlayers(SectionId center, int radius = 1)
        {
            var nearbyPlayers = new HashSet<int>();
            foreach (var section in center.GetNeighbors(radius).Append(center))
            {
                foreach (var playerId in GetPlayersInSection(section))
                {
                    nearbyPlayers.Add(playerId);
                }
            }
            return nearbyPlayers;
        }
    }

    // ===== 9. 확장된 SpatialManager =====

    public partial class SpatialManager
    {
        public int GetActiveSectionCount() => _sectionPlayers.Count;

        public Dictionary<SectionId, int> GetSectionPlayerCounts()
        {
            return _sectionPlayers.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray().Length
            );
        }

        // 핫스팟 섹션 감지 (플레이어가 많이 몰린 섹션)
        public IEnumerable<SectionId> GetHotspotSections(int threshold = 10)
        {
            return _sectionPlayers
                .Where(kvp => kvp.Value.ToArray().Length >= threshold)
                .Select(kvp => kvp.Key);
        }
    }
}
