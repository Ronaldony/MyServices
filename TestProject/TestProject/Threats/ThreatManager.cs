using TestProject.Entity;
using TestProject.Objects;
using TestProject.Utils;

namespace TestProject.Threats
{
    /// <summary>
    /// ThreatManager.
    /// </summary>
    public class ThreatManager
    {
        private readonly Dictionary<EntityId, ThreatEntry> _threatTable = new();
        private readonly object _lock = new object();

        public void AddThreat(IWorldObject target, float threat)
        {
            lock (_lock)
            {
                if (_threatTable.TryGetValue(target.Id, out var entry))
                {
                    entry.ThreatValue += threat;
                    entry.LastThreatTime = Time.time;
                    entry.LastKnownPosition = target.Position;
                }
                else
                {
                    _threatTable[target.Id] = new ThreatEntry
                    {
                        Target = target,
                        ThreatValue = threat,
                        LastThreatTime = Time.time,
                        LastKnownPosition = target.Position
                    };
                }
            }
        }

        public void RemoveThreat(EntityId targetId)
        {
            lock (_lock)
            {
                _threatTable.Remove(targetId);
            }
        }

        public void ClearAllThreat()
        {
            lock (_lock)
            {
                _threatTable.Clear();
            }
        }

        public IWorldObject GetHighestThreatTarget()
        {
            lock (_lock)
            {
                ThreatEntry highestThreat = null;
                foreach (var entry in _threatTable.Values)
                {
                    if (entry.Target != null && !entry.Target.IsInWorld) continue;

                    if (highestThreat == null || entry.ThreatValue > highestThreat.ThreatValue)
                    {
                        highestThreat = entry;
                    }
                }
                return highestThreat?.Target;
            }
        }

        public bool HasThreat => _threatTable.Count > 0;

        public List<IWorldObject> GetAllThreats()
        {
            lock (_lock)
            {
                return _threatTable.Values
                    .Where(t => t.Target != null && t.Target.IsInWorld)
                    .Select(t => t.Target)
                    .ToList();
            }
        }
    }
}
