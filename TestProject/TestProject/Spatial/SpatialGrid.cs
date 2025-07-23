using System.Numerics;
using TestProject.Objects;

namespace TestProject.Spatial
{
    /// <summary>
    /// SpatialGrid.
    /// </summary>
    public class SpatialGrid
    {
        /// <summary>
        /// 그리드.
        /// key: cell hash.
        /// value: 오브젝트.
        /// </summary>
        private readonly Dictionary<long, HashSet<IWorldObject>> _grid;
        private readonly float _cellSize;
        private readonly object _lock = new object();

        public SpatialGrid(float cellSize = 50f)
        {
            _grid = new Dictionary<long, HashSet<IWorldObject>>();
            _cellSize = cellSize;
        }

        public void AddObject(IWorldObject obj)
        {
            // 위치 해시 값.
            var cellKey = GetCellKey(obj.Position);

            lock (_lock)
            {
                // 
                if (!_grid.TryGetValue(cellKey, out var cell))
                {
                    cell = new HashSet<IWorldObject>();
                    _grid[cellKey] = cell;
                }
                cell.Add(obj);
            }
        }

        public void RemoveObject(IWorldObject obj)
        {
            var cellKey = GetCellKey(obj.Position);

            lock (_lock)
            {
                if (_grid.TryGetValue(cellKey, out var cell))
                {
                    cell.Remove(obj);
                    if (cell.Count == 0)
                    {
                        _grid.Remove(cellKey);
                    }
                }
            }
        }

        public void UpdateObjectPosition(IWorldObject obj, Vector3 oldPosition, Vector3 newPosition)
        {
            var oldCellKey = GetCellKey(oldPosition);
            var newCellKey = GetCellKey(newPosition);

            if (oldCellKey != newCellKey)
            {
                lock (_lock)
                {
                    // Remove from old cell
                    if (_grid.TryGetValue(oldCellKey, out var oldCell))
                    {
                        oldCell.Remove(obj);
                        if (oldCell.Count == 0)
                        {
                            _grid.Remove(oldCellKey);
                        }
                    }

                    // Add to new cell
                    if (!_grid.TryGetValue(newCellKey, out var newCell))
                    {
                        newCell = new HashSet<IWorldObject>();
                        _grid[newCellKey] = newCell;
                    }
                    newCell.Add(obj);
                }
            }
        }

        public List<IWorldObject> GetObjectsInRange(Vector3 position, float range)
        {
            var results = new List<IWorldObject>();
            var cellsToCheck = GetCellsInRange(position, range);

            lock (_lock)
            {
                foreach (var cellKey in cellsToCheck)
                {
                    if (_grid.TryGetValue(cellKey, out var cell))
                    {
                        foreach (var obj in cell)
                        {
                            if (Vector3.Distance(obj.Position, position) <= range)
                            {
                                results.Add(obj);
                            }
                        }
                    }
                }
            }

            return results;
        }

        private long GetCellKey(Vector3 position)
        {
            int x = (int)(position.X / _cellSize);
            int z = (int)(position.Z / _cellSize);
            return ((long)x << 32) | (uint)z;
        }

        private List<long> GetCellsInRange(Vector3 position, float range)
        {
            var cells = new List<long>();
            int cellRange = (int)Math.Ceiling(range / _cellSize);
            int centerX = (int)(position.X / _cellSize);
            int centerZ = (int)(position.Z / _cellSize);

            for (int x = centerX - cellRange; x <= centerX + cellRange; x++)
            {
                for (int z = centerZ - cellRange; z <= centerZ + cellRange; z++)
                {
                    long cellKey = ((long)x << 32) | (uint)z;
                    cells.Add(cellKey);
                }
            }

            return cells;
        }
    }
}
