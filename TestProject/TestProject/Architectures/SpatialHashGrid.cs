using System.Numerics;

namespace TestProject.Architectures
{
    /// <summary>
    /// 공간 해싱을 이용한 효율적인 충돌 감지 시스템
    /// </summary>
    public class SpatialHashGrid
    {
        private readonly Dictionary<int, List<ICollidable>> buckets;
        private readonly float cellSize;
        private readonly int tableSize;

        public SpatialHashGrid(float cellSize, int tableSize = 10007) // 소수 사용
        {
            this.cellSize = cellSize;
            this.tableSize = tableSize;
            buckets = new Dictionary<int, List<ICollidable>>();
        }

        /// <summary>
        /// 공간 해시 함수 (Matthias Müller의 해시 함수)
        /// </summary>
        private int HashCoords(int xi, int yi, int zi)
        {
            int h = xi * 73856093 ^ yi * 19349663 ^ zi * 83492791;
            return Math.Abs(h % tableSize);
        }

        /// <summary>
        /// 위치를 그리드 좌표로 변환
        /// </summary>
        private (int x, int y, int z) WorldToGrid(Vector3 position)
        {
            return (
                (int)Math.Floor(position.X / cellSize),
                (int)Math.Floor(position.Y / cellSize),
                (int)Math.Floor(position.Z / cellSize)
            );
        }

        /// <summary>
        /// 객체를 공간 해시에 추가
        /// </summary>
        public void Insert(ICollidable obj)
        {
            var cells = GetCellsForObject(obj);
            foreach (var cellHash in cells)
            {
                if (!buckets.ContainsKey(cellHash))
                {
                    buckets[cellHash] = new List<ICollidable>();
                }

                buckets[cellHash].Add(obj);
            }
        }

        /// <summary>
        /// 객체가 차지하는 모든 셀 계산
        /// </summary>
        private HashSet<int> GetCellsForObject(ICollidable obj)
        {
            HashSet<int> cells = new HashSet<int>();
            var bounds = obj.GetBounds();

            var minGrid = WorldToGrid(bounds.Min);
            var maxGrid = WorldToGrid(bounds.Max);

            for (int x = minGrid.x; x <= maxGrid.x; x++)
            {
                for (int y = minGrid.y; y <= maxGrid.y; y++)
                {
                    for (int z = minGrid.z; z <= maxGrid.z; z++)
                    {
                        cells.Add(HashCoords(x, y, z));
                    }
                }
            }

            return cells;
        }

        /// <summary>
        /// 특정 영역 내의 모든 객체 쿼리
        /// </summary>
        public HashSet<ICollidable> Query(ICollidable obj)
        {
            HashSet<ICollidable> nearbyObjects = new HashSet<ICollidable>();
            var cells = GetCellsForObject(obj);

            foreach (var cellHash in cells)
            {
                if (buckets.ContainsKey(cellHash))
                {
                    foreach (var other in buckets[cellHash])
                    {
                        if (other != obj)
                        {
                            nearbyObjects.Add(other);
                        }
                    }
                }
            }

            return nearbyObjects;
        }

        /// <summary>
        /// 연속 레이캐스트 (빠른 발사체용)
        /// </summary>
        public List<RaycastHit> Raycast(Vector3 start, Vector3 end, float radius)
        {
            var hits = new List<RaycastHit>();
            var direction = Vector3.Normalize(end - start);
            var distance = Vector3.Distance(start, end);

            // 브레젠험 3D 알고리즘으로 레이가 통과하는 셀 계산
            var cells = GetCellsAlongRay(start, end);
            var checkedObjects = new HashSet<ICollidable>();

            // start ~ end 사이 셀 영역을 탐색하여 충돌 객체 색출.
            foreach (var cellHash in cells)
            {
                // 버킷에 Cell
                if (buckets.ContainsKey(cellHash))
                {
                    foreach (var obj in buckets[cellHash])
                    {
                        if (checkedObjects.Contains(obj)) continue;
                        checkedObjects.Add(obj);

                        // 레이-구체 교차 검사
                        float? hitDistance = RaySphereIntersection(
                            start, direction, obj.Position, obj.Radius + radius);

                        if (hitDistance.HasValue && hitDistance.Value <= distance)
                        {
                            hits.Add(new RaycastHit
                            {
                                Object = obj,
                                Distance = hitDistance.Value,
                                Point = start + direction * hitDistance.Value
                            });
                        }
                    }
                }
            }

            // 거리순 정렬
            hits.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            return hits;
        }

        /// <summary>
        /// 레이가 통과하는 모든 셀 계산 (3D DDA 알고리즘)
        /// </summary>
        private List<int> GetCellsAlongRay(Vector3 start, Vector3 end)
        {
            var cells = new List<int>();

            var startGrid = WorldToGrid(start);
            var endGrid = WorldToGrid(end);

            // Grid.
            int x = startGrid.x;
            int y = startGrid.y;
            int z = startGrid.z;

            // Differential.
            int dx = Math.Abs(endGrid.x - startGrid.x);
            int dy = Math.Abs(endGrid.y - startGrid.y);
            int dz = Math.Abs(endGrid.z - startGrid.z);

            // 방향 계산.
            int stepX = startGrid.x < endGrid.x ? 1 : -1;
            int stepY = startGrid.y < endGrid.y ? 1 : -1;
            int stepZ = startGrid.z < endGrid.z ? 1 : -1;

            // 3D DDA 구현
            if (dx >= dy && dx >= dz)
            {
                // X가 주축
                int p1 = 2 * dy - dx;
                int p2 = 2 * dz - dx;

                while (x != endGrid.x)
                {
                    // 주축 x가 끝지점 그리드와 동일한 값이 될 때까지 셀 추가.
                    x += stepX;
                    if (p1 >= 0)
                    {
                        y += stepY;
                        p1 -= 2 * dx;
                    }
                    if (p2 >= 0)
                    {
                        z += stepZ;
                        p2 -= 2 * dx;
                    }

                    p1 += 2 * dy;
                    p2 += 2 * dz;

                    // 셀경로 해시값 저장.
                    cells.Add(HashCoords(x, y, z));
                }
            }
            // Y, Z 주축 케이스도 유사하게 구현...

            return cells;
        }

        /// <summary>
        /// 레이-구체 교차 검사
        /// </summary>
        private float? RaySphereIntersection(
            Vector3 rayOrigin, Vector3 rayDir, Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 oc = rayOrigin - sphereCenter;
            float a = Vector3.Dot(rayDir, rayDir);
            float b = 2.0f * Vector3.Dot(oc, rayDir);
            float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                return null;
            }

            float sqrt = (float)Math.Sqrt(discriminant);
            float t1 = (-b - sqrt) / (2 * a);
            float t2 = (-b + sqrt) / (2 * a);

            if (t1 >= 0) return t1;
            if (t2 >= 0) return t2;
            return null;
        }

        public void Clear()
        {
            buckets.Clear();
        }
    }

    /// <summary>
    /// 충돌 가능한 객체 인터페이스
    /// </summary>
    public interface ICollidable
    {
        int Id { get; }
        Vector3 Position { get; }
        float Radius { get; }
        BoundingBox GetBounds();
    }

    /// <summary>
    /// 경계 상자
    /// </summary>
    public struct BoundingBox
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public BoundingBox(Vector3 center, float radius)
        {
            Min = center - new Vector3(radius);
            Max = center + new Vector3(radius);
        }
    }

    /// <summary>
    /// 레이캐스트 결과
    /// </summary>
    public struct RaycastHit
    {
        public ICollidable Object { get; set; }
        public float Distance { get; set; }
        public Vector3 Point { get; set; }
    }
}