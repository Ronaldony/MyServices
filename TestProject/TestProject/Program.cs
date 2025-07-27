using System.Collections.Concurrent;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using TestProject.Entity;
using TestProject.Objects;

namespace MMORPGServer.GridSystem
{
    // ====================================================================
    // 그리드 시스템 기본 구조
    // ====================================================================

    public readonly struct GridCoord : IEquatable<GridCoord>
    {
        public readonly int X;
        public readonly int Y;

        public GridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(GridCoord other) => X == other.X && Y == other.Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";

        public static GridCoord operator +(GridCoord a, GridCoord b) => new(a.X + b.X, a.Y + b.Y);
        public static GridCoord operator -(GridCoord a, GridCoord b) => new(a.X - b.X, a.Y - b.Y);

        // 8방향 이웃 그리드
        public static readonly GridCoord[] AdjacentDirections = {
            new(-1, -1), new(-1, 0), new(-1, 1),
            new(0, -1),              new(0, 1),
            new(1, -1),  new(1, 0),  new(1, 1)
        };

        public IEnumerable<GridCoord> GetAdjacentCoordinates()
        {
            return AdjacentDirections.Select(dir => this + dir);
        }
    }

    public interface IGridEntity
    {
        EntityId Id { get; }
        Vector3 Position { get; set; }
        GridCoord CurrentGrid { get; set; }
        GridCoord PreviousGrid { get; set; }
        bool IsCrossGridEntity { get; } // 큰 엔티티가 여러 그리드에 걸쳐있는지
        HashSet<GridCoord> OccupiedGrids { get; }
    }

    // ====================================================================
    // Task Graph Engine 구현
    // ====================================================================

    public enum TaskType
    {
        EntityUpdate,        // 엔티티 업데이트
        CollisionDetection,  // 충돌 감지
        AIProcessing,        // AI 처리
        NetworkSync,         // 네트워크 동기화
        CrossGridSync,       // 크로스 그리드 동기화
        StateReplication     // 상태 복제
    }

    public interface IGridTask
    {
        string TaskId { get; }
        TaskType Type { get; }
        GridCoord TargetGrid { get; }
        HashSet<GridCoord> Dependencies { get; }
        int Priority { get; }
        TimeSpan EstimatedDuration { get; }

        Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public class TaskResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> OutputData { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
    }

    public class GridTaskGraph
    {
        private readonly ConcurrentDictionary<string, IGridTask> _tasks = new();
        private readonly ConcurrentDictionary<string, HashSet<string>> _dependencies = new();
        private readonly ConcurrentDictionary<string, HashSet<string>> _dependents = new();
        private readonly object _graphLock = new object();

        public void AddTask(IGridTask task)
        {
            lock (_graphLock)
            {
                _tasks[task.TaskId] = task;
                _dependencies[task.TaskId] = new HashSet<string>();
                _dependents[task.TaskId] = new HashSet<string>();
            }
        }

        public void AddDependency(string taskId, string dependsOnTaskId)
        {
            lock (_graphLock)
            {
                if (_tasks.ContainsKey(taskId) && _tasks.ContainsKey(dependsOnTaskId))
                {
                    _dependencies[taskId].Add(dependsOnTaskId);
                    _dependents[dependsOnTaskId].Add(taskId);
                }
            }
        }

        public async Task<Dictionary<string, TaskResult>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var results = new ConcurrentDictionary<string, TaskResult>();
            var completedTasks = new ConcurrentHashSet<string>();
            var executingTasks = new ConcurrentHashSet<string>();

            // 의존성이 없는 태스크들부터 시작
            var readyTasks = GetReadyTasks(completedTasks);

            while (readyTasks.Count > 0 || executingTasks.Count > 0)
            {
                // 준비된 태스크들을 병렬로 실행
                var executionTasks = readyTasks.Select(async taskId =>
                {
                    if (!executingTasks.Add(taskId)) return; // 이미 실행 중

                    try
                    {
                        var task = _tasks[taskId];
                        var startTime = DateTime.UtcNow;
                        var result = await task.ExecuteAsync(cancellationToken);
                        result.ExecutionTime = DateTime.UtcNow - startTime;

                        results[taskId] = result;
                        completedTasks.Add(taskId);
                    }
                    catch (Exception ex)
                    {
                        results[taskId] = new TaskResult
                        {
                            Success = false,
                            ErrorMessage = ex.Message,
                            ExecutionTime = TimeSpan.Zero
                        };
                        completedTasks.Add(taskId);
                    }
                    finally
                    {
                        executingTasks.Remove(taskId);
                    }
                }).ToArray();

                await Task.WhenAny(executionTasks);
                readyTasks = GetReadyTasks(completedTasks);
            }

            return new Dictionary<string, TaskResult>(results);
        }

        private HashSet<string> GetReadyTasks(ConcurrentHashSet<string> completedTasks)
        {
            var ready = new HashSet<string>();

            foreach (var taskId in _tasks.Keys)
            {
                // 이미 테크스를 저장한 경우.
                if (completedTasks.Contains(taskId)) continue;

                var dependencies = _dependencies[taskId];

                // 하위 의존성들이 completedTasks에 포함된 경우 -> 완료된 테스크인 경우.
                if (dependencies.All(dep => completedTasks.Contains(dep)))
                {
                    ready.Add(taskId);
                }
            }

            return ready;
        }
    }

    // ====================================================================
    // Grid Manager - TPL Dataflow 기반
    // ====================================================================

    public class GridManager
    {
        private readonly int _gridSize;
        private readonly ConcurrentDictionary<GridCoord, GridCell> _gridCells = new();
        private readonly GridTaskScheduler _taskScheduler;
        private readonly CrossGridSynchronizer _crossGridSync;

        // TPL Dataflow 파이프라인
        private  ActionBlock<EntityUpdateRequest> _entityUpdatePipeline;
        private ActionBlock<CrossGridEvent> _crossGridEventPipeline;
        private TransformBlock<GridCoord, GridUpdateResult> _gridUpdatePipeline;

        public GridManager(int gridSize = 100)
        {
            _gridSize = gridSize;
            _taskScheduler = new GridTaskScheduler();
            _crossGridSync = new CrossGridSynchronizer(this);

            InitializeDataflowPipeline();
        }

        private void InitializeDataflowPipeline()
        {
            // 엔티티 업데이트 파이프라인
            _entityUpdatePipeline = new ActionBlock<EntityUpdateRequest>(
                async request => await ProcessEntityUpdate(request),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = 10000,
                    CancellationToken = CancellationToken.None
                });

            // 크로스 그리드 이벤트 파이프라인
            _crossGridEventPipeline = new ActionBlock<CrossGridEvent>(
                async evt => await ProcessCrossGridEvent(evt),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 4, // 순서가 중요한 이벤트이므로 제한
                    BoundedCapacity = 5000
                });

            // 그리드 업데이트 파이프라인
            _gridUpdatePipeline = new TransformBlock<GridCoord, GridUpdateResult>(
                async gridCoord => await UpdateGrid(gridCoord),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
                    BoundedCapacity = 1000
                });
        }

        public GridCoord WorldToGrid(Vector3 worldPosition)
        {
            return new GridCoord(
                (int)Math.Floor(worldPosition.X / _gridSize),
                (int)Math.Floor(worldPosition.Z / _gridSize)
            );
        }

        public Vector3 GridToWorld(GridCoord gridCoord)
        {
            return new Vector3(
                gridCoord.X * _gridSize + _gridSize / 2f,
                0,
                gridCoord.Y * _gridSize + _gridSize / 2f
            );
        }

        public GridCell GetOrCreateGrid(GridCoord coord)
        {
            return _gridCells.GetOrAdd(coord, _ => new GridCell(coord, _gridSize));
        }

        public async Task ProcessEntityUpdateAsync(EntityUpdateRequest request)
        {
            await _entityUpdatePipeline.SendAsync(request);
        }

        private async Task ProcessEntityUpdate(EntityUpdateRequest request)
        {
            var entity = request.Entity;
            var newGridCoord = WorldToGrid(entity.Position);
            var oldGridCoord = entity.CurrentGrid;

            if (newGridCoord.Equals(oldGridCoord))
            {
                // 같은 그리드 내에서의 이동
                await UpdateEntityInGrid(entity);
            }
            else
            {
                // 그리드 경계 이동
                await HandleCrossGridMovement(entity, oldGridCoord, newGridCoord);
            }
        }

        private async Task HandleCrossGridMovement(IGridEntity entity, GridCoord oldGrid, GridCoord newGrid)
        {
            // 크로스 그리드 이벤트 생성
            var crossGridEvent = new CrossGridEvent
            {
                EventType = CrossGridEventType.EntityMoved,
                EntityId = entity.Id,
                SourceGrid = oldGrid,
                TargetGrid = newGrid,
                Entity = entity,
                Timestamp = DateTime.UtcNow
            };

            await _crossGridEventPipeline.SendAsync(crossGridEvent);
        }

        private async Task ProcessCrossGridEvent(CrossGridEvent evt)
        {
            switch (evt.EventType)
            {
                case CrossGridEventType.EntityMoved:
                    await _crossGridSync.HandleEntityMovement(evt);
                    break;
                case CrossGridEventType.SkillCast:
                    await _crossGridSync.HandleCrossGridSkill(evt);
                    break;
                case CrossGridEventType.DataSync:
                    await _crossGridSync.HandleDataSynchronization(evt);
                    break;
            }
        }

        private async Task UpdateEntityInGrid(IGridEntity entity)
        {
            var grid = GetOrCreateGrid(entity.CurrentGrid);
            await grid.UpdateEntity(entity);
        }

        private async Task<GridUpdateResult> UpdateGrid(GridCoord gridCoord)
        {
            var grid = GetOrCreateGrid(gridCoord);
            return await grid.ProcessUpdate();
        }

        // 병렬 그리드 업데이트 메인 루프
        public async Task UpdateAllGridsAsync(float deltaTime)
        {
            var activeGrids = _gridCells.Keys.ToList();

            // Task Graph 생성
            var taskGraph = new GridTaskGraph();
            var gridTasks = new Dictionary<GridCoord, List<IGridTask>>();

            // 각 그리드에 대한 태스크 생성
            foreach (var gridCoord in activeGrids)
            {
                var tasks = CreateGridTasks(gridCoord, deltaTime);
                gridTasks[gridCoord] = tasks;

                foreach (var task in tasks)
                {
                    taskGraph.AddTask(task);
                }
            }

            // 의존성 설정
            SetupTaskDependencies(taskGraph, gridTasks);

            // 병렬 실행
            var results = await taskGraph.ExecuteAsync();

            // 결과 처리
            await ProcessTaskResults(results);
        }

        private List<IGridTask> CreateGridTasks(GridCoord gridCoord, float deltaTime)
        {
            var tasks = new List<IGridTask>();
            var grid = GetOrCreateGrid(gridCoord);

            // 엔티티 업데이트 태스크
            tasks.Add(new EntityUpdateTask(gridCoord, grid, deltaTime));

            // AI 처리 태스크
            if (grid.HasAIEntities)
            {
                tasks.Add(new AIProcessingTask(gridCoord, grid, deltaTime));
            }

            // 충돌 감지 태스크
            if (grid.HasMovingEntities)
            {
                tasks.Add(new CollisionDetectionTask(gridCoord, grid, deltaTime));
            }

            // 네트워크 동기화 태스크
            if (grid.HasPlayerEntities)
            {
                tasks.Add(new NetworkSyncTask(gridCoord, grid));
            }

            return tasks;
        }

        private void SetupTaskDependencies(GridTaskGraph taskGraph, Dictionary<GridCoord, List<IGridTask>> gridTasks)
        {
            foreach (var gridCoord in gridTasks.Keys)
            {
                var tasks = gridTasks[gridCoord];

                // 같은 그리드 내 의존성 설정
                var entityTask = tasks.FirstOrDefault(t => t.Type == TaskType.EntityUpdate);
                var aiTask = tasks.FirstOrDefault(t => t.Type == TaskType.AIProcessing);
                var collisionTask = tasks.FirstOrDefault(t => t.Type == TaskType.CollisionDetection);
                var networkTask = tasks.FirstOrDefault(t => t.Type == TaskType.NetworkSync);

                if (entityTask != null && aiTask != null)
                {
                    // AI -> Entity 의존.
                    taskGraph.AddDependency(aiTask.TaskId, entityTask.TaskId);
                }

                if (aiTask != null && collisionTask != null)
                {
                    // Collision -> AI 의존.
                    taskGraph.AddDependency(collisionTask.TaskId, aiTask.TaskId);
                }

                if (collisionTask != null && networkTask != null)
                {
                    // Network -> Collision 의존.
                    taskGraph.AddDependency(networkTask.TaskId, collisionTask.TaskId);
                }

                // 결과적으로 Entity -> AI -> Collision -> Network Task 순으로 실행됨.
                // 인접 그리드와의 의존성 설정
                SetupCrossGridDependencies(taskGraph, gridCoord, gridTasks);
            }
        }

        private void SetupCrossGridDependencies(GridTaskGraph taskGraph, GridCoord currentGrid,
            Dictionary<GridCoord, List<IGridTask>> allGridTasks)
        {
            var currentTasks = allGridTasks[currentGrid];
            var collisionTask = currentTasks.FirstOrDefault(t => t.Type == TaskType.CollisionDetection);

            if (collisionTask == null) return;

            // 인접 그리드의 엔티티 업데이트가 완료된 후 충돌 감지 실행
            foreach (var adjacentGrid in currentGrid.GetAdjacentCoordinates())
            {
                if (allGridTasks.TryGetValue(adjacentGrid, out var adjacentTasks))
                {
                    var adjacentEntityTask = adjacentTasks.FirstOrDefault(t => t.Type == TaskType.EntityUpdate);
                    if (adjacentEntityTask != null)
                    {
                        taskGraph.AddDependency(collisionTask.TaskId, adjacentEntityTask.TaskId);
                    }
                }
            }
        }

        private async Task ProcessTaskResults(Dictionary<string, TaskResult> results)
        {
            var failedTasks = results.Where(r => !r.Value.Success).ToList();

            if (failedTasks.Any())
            {
                // 실패한 태스크 로깅 및 복구 처리
                foreach (var failedTask in failedTasks)
                {
                    Console.WriteLine($"Task {failedTask.Key} failed: {failedTask.Value.ErrorMessage}");
                }
            }

            // 성능 메트릭 수집
            var averageExecutionTime = results.Values.Average(r => r.ExecutionTime.TotalMilliseconds);
            var maxExecutionTime = results.Values.Max(r => r.ExecutionTime.TotalMilliseconds);

            // 성능 모니터링
            PerformanceMonitor.Instance.RecordGridUpdateMetrics(averageExecutionTime, maxExecutionTime);
        }
    }

    // ====================================================================
    // Grid Cell 구현
    // ====================================================================

    public class GridCell
    {
        public GridCoord Coordinate { get; }
        private readonly ConcurrentDictionary<EntityId, IGridEntity> _entities = new();
        private readonly ReaderWriterLockSlim _entityLock = new(LockRecursionPolicy.NoRecursion);
        private readonly int _gridSize;

        // 캐시된 상태 정보
        public bool HasAIEntities { get; private set; }
        public bool HasPlayerEntities { get; private set; }
        public bool HasMovingEntities { get; private set; }

        private DateTime _lastUpdate = DateTime.UtcNow;

        public GridCell(GridCoord coordinate, int gridSize)
        {
            Coordinate = coordinate;
            _gridSize = gridSize;
        }

        public async Task<GridUpdateResult> ProcessUpdate()
        {
            var result = new GridUpdateResult { GridCoordinate = Coordinate };

            try
            {
                _entityLock.EnterReadLock();

                // 상태 캐시 업데이트
                UpdateCachedStates();

                // 엔티티 업데이트 통계
                result.EntityCount = _entities.Count;
                result.Success = true;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                _entityLock.ExitReadLock();
            }
        }

        public async Task UpdateEntity(IGridEntity entity)
        {
            _entityLock.EnterWriteLock();
            try
            {
                _entities[entity.Id] = entity;
                entity.CurrentGrid = Coordinate;
                UpdateCachedStates();
            }
            finally
            {
                _entityLock.ExitWriteLock();
            }
        }

        public bool RemoveEntity(EntityId entityId)
        {
            _entityLock.EnterWriteLock();
            try
            {
                var removed = _entities.TryRemove(entityId, out _);
                if (removed)
                {
                    UpdateCachedStates();
                }
                return removed;
            }
            finally
            {
                _entityLock.ExitWriteLock();
            }
        }

        public List<IGridEntity> GetEntities()
        {
            _entityLock.EnterReadLock();
            try
            {
                return _entities.Values.ToList();
            }
            finally
            {
                _entityLock.ExitReadLock();
            }
        }

        public List<IGridEntity> GetEntitiesInRadius(Vector3 center, float radius)
        {
            _entityLock.EnterReadLock();
            try
            {
                var radiusSquared = radius * radius;
                return _entities.Values
                    .Where(entity => Vector3.DistanceSquared(entity.Position, center) <= radiusSquared)
                    .ToList();
            }
            finally
            {
                _entityLock.ExitReadLock();
            }
        }

        private void UpdateCachedStates()
        {
            HasAIEntities = _entities.Values.Any(e => e is Creature);
            HasPlayerEntities = _entities.Values.Any(e => e is Player);
            HasMovingEntities = _entities.Values.Any(e => Vector3.Distance(e.Position, e.Position) > 0.1f);
        }
    }

    // ====================================================================
    // Cross-Grid Synchronization
    // ====================================================================

    public enum CrossGridEventType
    {
        EntityMoved,
        SkillCast,
        DataSync,
        StateReplication
    }

    public class CrossGridEvent
    {
        public CrossGridEventType EventType { get; set; }
        public EntityId EntityId { get; set; }
        public GridCoord SourceGrid { get; set; }
        public GridCoord TargetGrid { get; set; }
        public IGridEntity Entity { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public class CrossGridSynchronizer
    {
        private readonly GridManager _gridManager;
        private readonly SemaphoreSlim _crossGridSemaphore;
        private readonly ConcurrentDictionary<EntityId, SemaphoreSlim> _entityLocks = new();

        public CrossGridSynchronizer(GridManager gridManager)
        {
            _gridManager = gridManager;
            _crossGridSemaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
        }

        public async Task HandleEntityMovement(CrossGridEvent evt)
        {
            var entityLock = _entityLocks.GetOrAdd(evt.EntityId, _ => new SemaphoreSlim(1, 1));

            await entityLock.WaitAsync();
            try
            {
                await _crossGridSemaphore.WaitAsync();
                try
                {
                    await PerformAtomicGridTransfer(evt.Entity, evt.SourceGrid, evt.TargetGrid);
                }
                finally
                {
                    _crossGridSemaphore.Release();
                }
            }
            finally
            {
                entityLock.Release();
            }
        }

        private async Task PerformAtomicGridTransfer(IGridEntity entity, GridCoord sourceGrid, GridCoord targetGrid)
        {
            // Phase 1: 새 그리드에 추가
            var targetGridCell = _gridManager.GetOrCreateGrid(targetGrid);
            await targetGridCell.UpdateEntity(entity);

            // Phase 2: 이전 그리드에서 제거
            var sourceGridCell = _gridManager.GetOrCreateGrid(sourceGrid);
            sourceGridCell.RemoveEntity(entity.Id);

            // Phase 3: 엔티티 상태 업데이트
            entity.PreviousGrid = sourceGrid;
            entity.CurrentGrid = targetGrid;

            // Phase 4: 인접 그리드에 알림
            await NotifyAdjacentGridsOfMovement(entity, sourceGrid, targetGrid);
        }

        private async Task NotifyAdjacentGridsOfMovement(IGridEntity entity, GridCoord sourceGrid, GridCoord targetGrid)
        {
            var affectedGrids = new HashSet<GridCoord>();

            // 소스 그리드 인접
            affectedGrids.UnionWith(sourceGrid.GetAdjacentCoordinates());

            // 타겟 그리드 인접
            affectedGrids.UnionWith(targetGrid.GetAdjacentCoordinates());

            var notificationTasks = affectedGrids.Select(async gridCoord =>
            {
                var gridCell = _gridManager.GetOrCreateGrid(gridCoord);
                var nearbyEntities = gridCell.GetEntitiesInRadius(entity.Position, 50f);

                // 가시성 업데이트가 필요한 엔티티들에게 알림
                foreach (var nearbyEntity in nearbyEntities)
                {
                    if (nearbyEntity is Player player)
                    {
                        await NotifyPlayerOfEntityMovement(player, entity);
                    }
                }
            });

            await Task.WhenAll(notificationTasks);
        }

        private async Task NotifyPlayerOfEntityMovement(Player player, IGridEntity movedEntity)
        {
            // 플레이어에게 엔티티 이동 패킷 전송
            var packet = new EntityMovementPacket
            {
                EntityId = movedEntity.Id,
                Position = movedEntity.Position,
                GridCoordinate = movedEntity.CurrentGrid
            };

            await NetworkManager.Instance.SendToPlayer(player.Id, packet);
        }

        public async Task HandleCrossGridSkill(CrossGridEvent evt)
        {
            // 크로스 그리드 스킬 캐스팅 처리
            var affectedGrids = CalculateAffectedGrids(evt);

            await using var coordinator = new CrossGridCoordinator(affectedGrids);
            await coordinator.ExecuteAtomicOperation(async () =>
            {
                // 모든 영향받는 그리드에서 스킬 효과 적용
                await ApplySkillEffectAcrossGrids(evt, affectedGrids);
            });
        }

        private HashSet<GridCoord> CalculateAffectedGrids(CrossGridEvent evt)
        {
            var affectedGrids = new HashSet<GridCoord>();

            if (evt.Data.TryGetValue("SkillRadius", out var radiusObj) && radiusObj is float radius)
            {
                var centerGrid = evt.SourceGrid;
                var gridRadius = (int)Math.Ceiling(radius / 100f); // 그리드 크기가 100이라 가정

                for (int x = centerGrid.X - gridRadius; x <= centerGrid.X + gridRadius; x++)
                {
                    for (int y = centerGrid.Y - gridRadius; y <= centerGrid.Y + gridRadius; y++)
                    {
                        affectedGrids.Add(new GridCoord(x, y));
                    }
                }
            }

            return affectedGrids;
        }

        private async Task ApplySkillEffectAcrossGrids(CrossGridEvent evt, HashSet<GridCoord> affectedGrids)
        {
            var tasks = affectedGrids.Select(async gridCoord =>
            {
                var gridCell = _gridManager.GetOrCreateGrid(gridCoord);
                var entities = gridCell.GetEntities();

                foreach (var entity in entities)
                {
                    // 스킬 범위 내 엔티티에게 효과 적용
                    if (IsEntityInSkillRange(entity, evt))
                    {
                        await ApplySkillEffect(entity, evt);
                    }
                }
            });

            await Task.WhenAll(tasks);
        }

        private bool IsEntityInSkillRange(IGridEntity entity, CrossGridEvent evt)
        {
            if (evt.Data.TryGetValue("SkillCenter", out var centerObj) && centerObj is Vector3 center &&
                evt.Data.TryGetValue("SkillRadius", out var radiusObj) && radiusObj is float radius)
            {
                return Vector3.Distance(entity.Position, center) <= radius;
            }

            return false;
        }

        private async Task ApplySkillEffect(IGridEntity entity, CrossGridEvent evt)
        {
            // 스킬 효과 적용 로직
            if (evt.Data.TryGetValue("Damage", out var damageObj) && damageObj is float damage)
            {
                if (entity is Player player)
                {
                    player.Health.TakeDamage(damage);
                }
                else if (entity is Creature creature)
                {
                    creature.Health.TakeDamage(damage);
                }
            }
        }

        public async Task HandleDataSynchronization(CrossGridEvent evt)
        {
            // 그리드 간 데이터 동기화 처리
            var sourceData = await GetGridData(evt.SourceGrid);
            await SynchronizeGridData(evt.TargetGrid, sourceData);
        }

        private async Task<GridData> GetGridData(GridCoord gridCoord)
        {
            var gridCell = _gridManager.GetOrCreateGrid(gridCoord);
            var entities = gridCell.GetEntities();

            return new GridData
            {
                GridCoordinate = gridCoord,
                EntityStates = entities.ToDictionary(e => e.Id, e => new EntityState
                {
                    Position = e.Position,
                    GridCoordinate = e.CurrentGrid
                }),
                Timestamp = DateTime.UtcNow
            };
        }

        private async Task SynchronizeGridData(GridCoord targetGrid, GridData sourceData)
        {
            var targetGridCell = _gridManager.GetOrCreateGrid(targetGrid);

            // 데이터 동기화 로직
            foreach (var entityState in sourceData.EntityStates.Values)
            {
                // 필요한 경우 엔티티 상태 업데이트
                await UpdateEntityStateIfNeeded(targetGridCell, entityState);
            }
        }

        private async Task UpdateEntityStateIfNeeded(GridCell gridCell, EntityState entityState)
        {
            var entities = gridCell.GetEntities();
            var entity = entities.FirstOrDefault(e => e.Id == entityState.EntityId);

            if (entity != null)
            {
                // 위치가 다르면 업데이트
                if (Vector3.Distance(entity.Position, entityState.Position) > 0.1f)
                {
                    entity.Position = entityState.Position;
                    await gridCell.UpdateEntity(entity);
                }
            }
        }
    }

    // ====================================================================
    // Cross-Grid Coordinator (원자적 연산 보장)
    // ====================================================================

    public class CrossGridCoordinator : IAsyncDisposable
    {
        private readonly HashSet<GridCoord> _lockedGrids;
        private readonly SemaphoreSlim[] _gridLocks;
        private readonly ConcurrentDictionary<GridCoord, SemaphoreSlim> _globalGridLocks;
        private static readonly ConcurrentDictionary<GridCoord, SemaphoreSlim> GlobalLocks = new();

        public CrossGridCoordinator(HashSet<GridCoord> gridsToLock)
        {
            _lockedGrids = gridsToLock.OrderBy(g => g.GetHashCode()).ToHashSet(); // 데드락 방지를 위한 정렬
            _gridLocks = new SemaphoreSlim[_lockedGrids.Count];

            // 글로벌 그리드 락 획득
            var lockTasks = _lockedGrids.Select(async (grid, index) =>
            {
                var gridLock = GlobalLocks.GetOrAdd(grid, _ => new SemaphoreSlim(1, 1));
                await gridLock.WaitAsync();
                _gridLocks[index] = gridLock;
            });

            Task.WaitAll(lockTasks.ToArray());
        }

        public async Task ExecuteAtomicOperation(Func<Task> operation)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                // 예외 발생 시 롤백 로직
                Console.WriteLine($"Cross-grid operation failed: {ex.Message}");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            // 모든 락 해제 (획득 순서의 역순으로)
            for (int i = _gridLocks.Length - 1; i >= 0; i--)
            {
                _gridLocks[i]?.Release();
            }
        }
    }

    // ====================================================================
    // Task 구현체들
    // ====================================================================

    public abstract class BaseGridTask : IGridTask
    {
        public string TaskId { get; protected set; }
        public abstract TaskType Type { get; }
        public GridCoord TargetGrid { get; protected set; }
        public HashSet<GridCoord> Dependencies { get; protected set; } = new();
        public int Priority { get; protected set; } = 0;
        public TimeSpan EstimatedDuration { get; protected set; } = TimeSpan.FromMilliseconds(5);

        protected BaseGridTask(GridCoord targetGrid)
        {
            TargetGrid = targetGrid;
            TaskId = $"{Type}_{targetGrid}_{Guid.NewGuid():N}";
        }

        public abstract Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public class EntityUpdateTask : BaseGridTask
    {
        public override TaskType Type => TaskType.EntityUpdate;
        private readonly GridCell _gridCell;
        private readonly float _deltaTime;

        public EntityUpdateTask(GridCoord targetGrid, GridCell gridCell, float deltaTime)
            : base(targetGrid)
        {
            _gridCell = gridCell;
            _deltaTime = deltaTime;
            Priority = 10; // 높은 우선순위
        }

        public override async Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var entities = _gridCell.GetEntities();

                var updateTasks = entities.Select(async entity =>
                {
                    // 엔티티별 업데이트 로직
                    if (entity is Player player)
                    {
                        await UpdatePlayer(player, _deltaTime);
                    }
                    else if (entity is Creature creature)
                    {
                        await UpdateCreature(creature, _deltaTime);
                    }
                });

                await Task.WhenAll(updateTasks);

                return new TaskResult
                {
                    Success = true,
                    OutputData = { ["UpdatedEntityCount"] = entities.Count }
                };
            }
            catch (Exception ex)
            {
                return new TaskResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task UpdatePlayer(Player player, float deltaTime)
        {
            // 플레이어 업데이트 로직
            player.Update(deltaTime);
        }

        private async Task UpdateCreature(Creature creature, float deltaTime)
        {
            // 크리쳐 업데이트 로직
            creature.Update(deltaTime);
        }
    }

    public class AIProcessingTask : BaseGridTask
    {
        public override TaskType Type => TaskType.AIProcessing;
        private readonly GridCell _gridCell;
        private readonly float _deltaTime;

        public AIProcessingTask(GridCoord targetGrid, GridCell gridCell, float deltaTime)
            : base(targetGrid)
        {
            _gridCell = gridCell;
            _deltaTime = deltaTime;
            Priority = 8;
        }

        public override async Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var creatures = _gridCell.GetEntities().OfType<Creature>().ToList();

                await Task.Run(() =>
                {
                    Parallel.ForEach(creatures, creature =>
                    {
                        creature.AI.Update(_deltaTime);
                    });
                });

                return new TaskResult
                {
                    Success = true,
                    OutputData = { ["ProcessedAICount"] = creatures.Count }
                };
            }
            catch (Exception ex)
            {
                return new TaskResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    public class CollisionDetectionTask : BaseGridTask
    {
        public override TaskType Type => TaskType.CollisionDetection;
        private readonly GridCell _gridCell;
        private readonly float _deltaTime;

        public CollisionDetectionTask(GridCoord targetGrid, GridCell gridCell, float deltaTime)
            : base(targetGrid)
        {
            _gridCell = gridCell;
            _deltaTime = deltaTime;
            Priority = 6;
            EstimatedDuration = TimeSpan.FromMilliseconds(10);
        }

        public override async Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var entities = _gridCell.GetEntities();
                var collisionPairs = new List<(IGridEntity, IGridEntity)>();

                // 간단한 O(n²) 충돌 검사 (실제로는 더 최적화된 알고리즘 사용)
                for (int i = 0; i < entities.Count; i++)
                {
                    for (int j = i + 1; j < entities.Count; j++)
                    {
                        if (CheckCollision(entities[i], entities[j]))
                        {
                            collisionPairs.Add((entities[i], entities[j]));
                        }
                    }
                }

                // 충돌 처리
                foreach (var (entity1, entity2) in collisionPairs)
                {
                    await HandleCollision(entity1, entity2);
                }

                return new TaskResult
                {
                    Success = true,
                    OutputData = { ["CollisionCount"] = collisionPairs.Count }
                };
            }
            catch (Exception ex)
            {
                return new TaskResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private bool CheckCollision(IGridEntity entity1, IGridEntity entity2)
        {
            float distance = Vector3.Distance(entity1.Position, entity2.Position);
            return distance < 2.0f; // 간단한 거리 기반 충돌
        }

        private async Task HandleCollision(IGridEntity entity1, IGridEntity entity2)
        {
            // 충돌 처리 로직
            await Task.CompletedTask;
        }
    }

    public class NetworkSyncTask : BaseGridTask
    {
        public override TaskType Type => TaskType.NetworkSync;
        private readonly GridCell _gridCell;

        public NetworkSyncTask(GridCoord targetGrid, GridCell gridCell)
            : base(targetGrid)
        {
            _gridCell = gridCell;
            Priority = 4;
        }

        public override async Task<TaskResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var players = _gridCell.GetEntities().OfType<Player>().ToList();
                var syncTasks = new List<Task>();

                foreach (var player in players)
                {
                    syncTasks.Add(SyncPlayerData(player));
                }

                await Task.WhenAll(syncTasks);

                return new TaskResult
                {
                    Success = true,
                    OutputData = { ["SyncedPlayerCount"] = players.Count }
                };
            }
            catch (Exception ex)
            {
                return new TaskResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task SyncPlayerData(Player player)
        {
            // 플레이어 데이터 동기화
            var nearbyEntities = _gridCell.GetEntitiesInRadius(player.Position, 50f);

            var updatePacket = new GridUpdatePacket
            {
                PlayerId = player.Id,
                GridCoordinate = TargetGrid,
                NearbyEntities = nearbyEntities.Select(e => new EntitySnapshot
                {
                    EntityId = e.Id,
                    Position = e.Position,
                    GridCoordinate = e.CurrentGrid
                }).ToList()
            };

            await NetworkManager.Instance.SendToPlayer(player.Id, updatePacket);
        }
    }

    // ====================================================================
    // 지원 클래스들
    // ====================================================================

    public class GridTaskScheduler
    {
        private readonly TaskScheduler _scheduler;
        private readonly SemaphoreSlim _concurrencyLimiter;

        public GridTaskScheduler()
        {
            _scheduler = TaskScheduler.Default;
            _concurrencyLimiter = new SemaphoreSlim(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        }

        public async Task<T> ScheduleTask<T>(Func<Task<T>> taskFactory, TaskPriority priority = TaskPriority.Normal)
        {
            await _concurrencyLimiter.WaitAsync();
            try
            {
                return await Task.Factory.StartNew(async () => await taskFactory(),
                    CancellationToken.None,
                    GetTaskCreationOptions(priority),
                    _scheduler).Unwrap();
            }
            finally
            {
                _concurrencyLimiter.Release();
            }
        }

        private TaskCreationOptions GetTaskCreationOptions(TaskPriority priority)
        {
            return priority switch
            {
                TaskPriority.High => TaskCreationOptions.PreferFairness,
                TaskPriority.Low => TaskCreationOptions.LongRunning,
                _ => TaskCreationOptions.None
            };
        }
    }

    public enum TaskPriority
    {
        Low,
        Normal,
        High
    }

    // 결과 및 상태 클래스들
    public class GridUpdateResult
    {
        public GridCoord GridCoordinate { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int EntityCount { get; set; }
        public TimeSpan UpdateDuration { get; set; }
    }

    public class EntityUpdateRequest
    {
        public IGridEntity Entity { get; set; }
        public Vector3 NewPosition { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class GridData
    {
        public GridCoord GridCoordinate { get; set; }
        public Dictionary<EntityId, EntityState> EntityStates { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    public class EntityState
    {
        public EntityId EntityId { get; set; }
        public Vector3 Position { get; set; }
        public GridCoord GridCoordinate { get; set; }
    }

    // 네트워크 패킷들
    public class EntityMovementPacket : GamePacket
    {
        public override PacketType Type => PacketType.EntityMovement;
        public EntityId EntityId { get; set; }
        public Vector3 Position { get; set; }
        public GridCoord GridCoordinate { get; set; }

        public override void SerializeToStream(BinaryWriter writer)
        {
            writer.Write(EntityId.Value);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);
            writer.Write(GridCoordinate.X);
            writer.Write(GridCoordinate.Y);
        }

        public override void DeserializeFromStream(BinaryReader reader)
        {
            EntityId = new EntityId(reader.ReadUInt32());
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            GridCoordinate = new GridCoord(reader.ReadInt32(), reader.ReadInt32());
        }
    }

    public class GridUpdatePacket : GamePacket
    {
        public override PacketType Type => PacketType.GridUpdate;
        public EntityId PlayerId { get; set; }
        public GridCoord GridCoordinate { get; set; }
        public List<EntitySnapshot> NearbyEntities { get; set; } = new();

        public override void SerializeToStream(BinaryWriter writer)
        {
            writer.Write(PlayerId.Value);
            writer.Write(GridCoordinate.X);
            writer.Write(GridCoordinate.Y);
            writer.Write(NearbyEntities.Count);

            foreach (var entity in NearbyEntities)
            {
                writer.Write(entity.EntityId.Value);
                writer.Write(entity.Position.X);
                writer.Write(entity.Position.Y);
                writer.Write(entity.Position.Z);
                writer.Write(entity.GridCoordinate.X);
                writer.Write(entity.GridCoordinate.Y);
            }
        }

        public override void DeserializeFromStream(BinaryReader reader)
        {
            PlayerId = new EntityId(reader.ReadUInt32());
            GridCoordinate = new GridCoord(reader.ReadInt32(), reader.ReadInt32());
            int entityCount = reader.ReadInt32();

            NearbyEntities = new List<EntitySnapshot>(entityCount);
            for (int i = 0; i < entityCount; i++)
            {
                NearbyEntities.Add(new EntitySnapshot
                {
                    EntityId = new EntityId(reader.ReadUInt32()),
                    Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    GridCoordinate = new GridCoord(reader.ReadInt32(), reader.ReadInt32())
                });
            }
        }
    }

    public class EntitySnapshot
    {
        public EntityId EntityId { get; set; }
        public Vector3 Position { get; set; }
        public GridCoord GridCoordinate { get; set; }
    }

    // Thread-safe 컬렉션
    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary = new();

        public bool Add(T item) => _dictionary.TryAdd(item, 0);
        public bool Remove(T item) => _dictionary.TryRemove(item, out _);
        public bool Contains(T item) => _dictionary.ContainsKey(item);
        public void Clear() => _dictionary.Clear();
        public int Count => _dictionary.Count;

        public IEnumerator<T> GetEnumerator() => _dictionary.Keys.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // 확장된 패킷 타입
    public enum PacketType : byte
    {
        EntityMovement = 60,
        GridUpdate = 61,
        CrossGridEvent = 62
    }

    // 성능 모니터링
    public class PerformanceMonitor
    {
        private static readonly Lazy<PerformanceMonitor> _instance = new(() => new PerformanceMonitor());
        public static PerformanceMonitor Instance => _instance.Value;

        public void RecordGridUpdateMetrics(double averageExecutionTime, double maxExecutionTime)
        {
            // 성능 메트릭 기록
            Console.WriteLine($"Grid Update - Avg: {averageExecutionTime:F2}ms, Max: {maxExecutionTime:F2}ms");
        }
    }

    // ====================================================================
    // 사용 예제
    // ====================================================================

    public class GridSystemExample
    {
        private readonly GridManager _gridManager;

        public GridSystemExample()
        {
            _gridManager = new GridManager(100); // 100x100 그리드 크기
        }

        public async Task RunExample()
        {
            // 게임 루프 시뮬레이션
            for (int frame = 0; frame < 1000; frame++)
            {
                await _gridManager.UpdateAllGridsAsync(0.016f); // 60 FPS

                if (frame % 60 == 0) // 1초마다
                {
                    Console.WriteLine($"Frame {frame}: Grid system updated");
                }
            }
        }

        public async Task TestCrossGridMovement()
        {
            // 엔티티 생성
            var player = new Player(new EntityId(1))
            {
                Position = new Vector3(95, 0, 95), // 그리드 경계 근처
                CurrentGrid = _gridManager.WorldToGrid(new Vector3(95, 0, 95))
            };

            // 그리드 경계를 넘나드는 이동 테스트
            for (int i = 0; i < 20; i++)
            {
                player.Position += new Vector3(1, 0, 1);

                var updateRequest = new EntityUpdateRequest
                {
                    Entity = player,
                    NewPosition = player.Position,
                    Timestamp = DateTime.UtcNow
                };

                await _gridManager.ProcessEntityUpdateAsync(updateRequest);

                Console.WriteLine($"Player moved to {player.Position}, Grid: {player.CurrentGrid}");
                await Task.Delay(100);
            }
        }
    }
}