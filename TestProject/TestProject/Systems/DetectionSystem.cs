using System.Numerics;
using TestProject.Managers;
using TestProject.Objects;
using TestProject.Types;
using TestProject.Utils;

namespace TestProject.Systems
{
    /// <summary>
    /// DetectionSystem.
    /// </summary>
    public class DetectionSystem
    {
        private readonly Creature _owner;

        public float SightRange { get; set; } = 25f;
        public float AggroRange { get; set; } = 15f;
        public float CallForHelpRange { get; set; } = 20f;
        public float FieldOfView { get; set; } = 180f; // Degrees

        private readonly HashSet<IWorldObject> _detectedEnemies = new();
        private float _lastDetectionUpdate;

        public DetectionSystem(Creature owner)
        {
            _owner = owner;
        }

        public void Update(float deltaTime)
        {
            // Update detection every 200ms for performance
            if (Time.time - _lastDetectionUpdate < 0.2f) return;
            _lastDetectionUpdate = Time.time;

            _detectedEnemies.Clear();

            // Get nearby objects from spatial system
            var nearbyObjects = WorldManager.Instance.GetObjectsInRange(_owner.Position, SightRange);

            foreach (var obj in nearbyObjects)
            {
                if (obj == _owner || obj.ObjectType != ObjectType.Player) continue;

                if (CanDetect(obj))
                {
                    _detectedEnemies.Add(obj);
                }
            }
        }

        private bool CanDetect(IWorldObject target)
        {
            float distance = Vector3.Distance(_owner.Position, target.Position);

            // Check range
            if (distance > SightRange) return false;

            // Check field of view
            var directionToTarget = Vector3.Normalize(target.Position - _owner.Position);
            var forward = _owner.GetForwardVector();
            var angle = Math.Acos(Vector3.Dot(forward, directionToTarget)) * (180.0 / Math.PI);

            if (angle > FieldOfView / 2) return false;

            // Check line of sight (simplified - in real implementation would use ray casting)
            return HasLineOfSight(target);
        }

        private bool HasLineOfSight(IWorldObject target)
        {
            // Simplified LOS check - in real implementation would raycast against terrain
            return true;
        }

        public List<IWorldObject> GetDetectedEnemies() => _detectedEnemies.ToList();

        public IWorldObject GetClosestEnemy(List<IWorldObject> enemies)
        {
            if (enemies.Count == 0) return null;

            IWorldObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(_owner.Position, enemy.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }

            return closest;
        }

        public bool IsInAggroRange(IWorldObject target)
        {
            return Vector3.Distance(_owner.Position, target.Position) <= AggroRange;
        }
    }

}
