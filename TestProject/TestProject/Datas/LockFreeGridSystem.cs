using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TestProject.Datas
{
    /// <summary>
    // Lock-free Grid System
    /// LockFreeGridSystem.
    /// </summary>
    public class LockFreeGridSystem
    {
        private readonly float _cellSize;
        private readonly ConcurrentDictionary<(int, int, int), ImmutableHashSet<Guid>> _grid;
        private readonly Subject<GridUpdateEvent> _gridEvents;

        public record GridUpdateEvent(Guid EntityId, (int, int, int) OldCell, (int, int, int) NewCell);

        public IObservable<GridUpdateEvent> GridUpdates => _gridEvents.AsObservable();

        public LockFreeGridSystem(float cellSize = 100f)
        {
            _cellSize = cellSize;
            _grid = new ConcurrentDictionary<(int, int, int), ImmutableHashSet<Guid>>();
            _gridEvents = new Subject<GridUpdateEvent>();
        }

        private (int, int, int) GetCellCoordinates(Vector3 position)
        {
            return (
                (int)(position.X / _cellSize),
                (int)(position.Y / _cellSize),
                (int)(position.Z / _cellSize)
            );
        }

        public void UpdateEntityPosition(Guid entityId, Vector3 oldPosition, Vector3 newPosition)
        {
            var oldCell = GetCellCoordinates(oldPosition);
            var newCell = GetCellCoordinates(newPosition);

            if (oldCell != newCell)
            {
                // Remove from old cell
                _grid.AddOrUpdate(oldCell,
                    _ => ImmutableHashSet<Guid>.Empty,
                    (_, set) => set.Remove(entityId));

                // Add to new cell
                _grid.AddOrUpdate(newCell,
                    _ => ImmutableHashSet<Guid>.Empty.Add(entityId),
                    (_, set) => set.Add(entityId));

                _gridEvents.OnNext(new GridUpdateEvent(entityId, oldCell, newCell));
            }
        }

        public async IAsyncEnumerable<Guid> GetNearbyEntitiesAsync(Vector3 position, float radius)
        {
            var centerCell = GetCellCoordinates(position);
            var cellRadius = (int)Math.Ceiling(radius / _cellSize);

            var cells = new List<(int, int, int)>();

            for (int dx = -cellRadius; dx <= cellRadius; dx++)
                for (int dy = -cellRadius; dy <= cellRadius; dy++)
                    for (int dz = -cellRadius; dz <= cellRadius; dz++)
                    {
                        cells.Add((centerCell.Item1 + dx, centerCell.Item2 + dy, centerCell.Item3 + dz));
                    }

            foreach (var cell in cells)
            {
                if (_grid.TryGetValue(cell, out var entities))
                {
                    foreach (var entityId in entities)
                    {
                        yield return entityId;
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}
