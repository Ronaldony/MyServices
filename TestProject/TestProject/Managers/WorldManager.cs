using System.Numerics;
using TestProject.Entity;
using TestProject.Objects;
using TestProject.Spatial;

namespace TestProject.Managers
{
    /// <summary>
    /// WorldManager.
    /// </summary>
    public class WorldManager
    {
        private static WorldManager _instance;
        public static WorldManager Instance => _instance ??= new WorldManager();

        private readonly Dictionary<EntityId, IWorldObject> _objects = new();
        private readonly SpatialGrid _spatialGrid = new();
        private readonly object _lock = new object();

        public void AddObject(IWorldObject obj)
        {
            lock (_lock)
            {
                _objects[obj.Id] = obj;
                _spatialGrid.AddObject(obj);
            }
        }

        public void RemoveObject(EntityId id)
        {
            lock (_lock)
            {
                if (_objects.TryGetValue(id, out var obj))
                {
                    _spatialGrid.RemoveObject(obj);
                    _objects.Remove(id);
                }
            }
        }

        public void UpdateObjectPosition(IWorldObject obj, Vector3 oldPosition, Vector3 newPosition)
        {
            _spatialGrid.UpdateObjectPosition(obj, oldPosition, newPosition);
        }

        public List<IWorldObject> GetObjectsInRange(Vector3 position, float range)
        {
            return _spatialGrid.GetObjectsInRange(position, range);
        }

        public IWorldObject GetObject(EntityId id)
        {
            lock (_lock)
            {
                _objects.TryGetValue(id, out var obj);
                return obj;
            }
        }
    }
}
